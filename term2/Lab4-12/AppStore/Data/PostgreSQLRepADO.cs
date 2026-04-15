using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Npgsql;

namespace Project.Data
{
    public class PostgreSQLRepADO : BaseRepository
    {
        private readonly string _connectionString;
        private readonly Guid _userId;

        public PostgreSQLRepADO(Guid currentUserId = default)
        {
            _userId = currentUserId;
            _connectionString = LoadConnectionString();

            EnsureDatabaseCreated();
            EnsureSeeded();
        }

        public static string LoadConnectionString()
        {
            var envPath = Path.Combine(AppContext.BaseDirectory, ".env");
            if (!File.Exists(envPath))
                envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");

            string host = "localhost",
                port = "5433",
                database = "postgres",
                username = "postgres",
                password = "111";

            if (File.Exists(envPath))
            {
                foreach (var raw in File.ReadAllLines(envPath))
                {
                    var line = raw.Trim();
                    if (line.StartsWith('#') || !line.Contains('='))
                        continue;

                    var eqIdx = line.IndexOf('=');
                    var key = line[..eqIdx].Trim();
                    var val = line[(eqIdx + 1)..].Trim().Trim('"');

                    switch (key)
                    {
                        case "DB_HOST":
                            host = val;
                            break;
                        case "DB_PORT_FROM":
                            port = val;
                            break;
                        case "DB_NAME":
                            database = val;
                            break;
                        case "DB_ADMIN":
                            username = val;
                            break;
                        case "DB_ADMIN_PASS":
                            password = val;
                            break;
                    }
                }
            }

            return $"Host={host};Port={port};Database={database};Username={username};Password={password}";
        }

        public override List<App> GetAllApps()
        {
            var apps = new List<App>();
            var installedIds = GetInstalledAppIds();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            const string sql = """
                    select id, short_name, full_name, description, developer, category,
                           rating, rating_count, price, version, size_mb, country,
                           age_rating, color, is_featured, is_in_stock, download_count,
                           discount_percent, release_date, tags
                    from apps
                    order by full_name
                """;

            using var command = new NpgsqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var app = MapApp(reader);
                app.IsDownloaded = installedIds.Contains(app.Id);
                apps.Add(app);
            }

            return apps;
        }

        public override App? GetAppById(Guid id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            const string sql = """
                    select id, short_name, full_name, description, developer, category,
                           rating, rating_count, price, version, size_mb, country,
                           age_rating, color, is_featured, is_in_stock, download_count,
                           discount_percent, release_date, tags
                    from apps
                    where id = @id
                    limit 1
                """;

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("id", id);

            using var reader = command.ExecuteReader();
            if (!reader.Read())
                return null;

            var app = MapApp(reader);
            app.IsDownloaded = IsAppDownloaded(id);

            return app;
        }

        public override void AddApp(App app)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            const string sql = """
                    insert into apps
                    (
                        id, short_name, full_name, description, developer, category,
                        rating, rating_count, price, version, size_mb, country,
                        age_rating, color, is_featured, is_in_stock, download_count,
                        discount_percent, release_date, tags
                    )
                    values
                    (
                        @id, @short_name, @full_name, @description, @developer, @category,
                        @rating, @rating_count, @price, @version, @size_mb, @country,
                        @age_rating, @color, @is_featured, @is_in_stock, @download_count,
                        @discount_percent, @release_date, @tags
                    )
                """;

            using var command = new NpgsqlCommand(sql, connection);
            FillAppParameters(command, app);
            command.ExecuteNonQuery();
        }

        public override void UpdateApp(App app)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            const string sql = """
                    update apps
                    set short_name = @short_name,
                        full_name = @full_name,
                        description = @description,
                        developer = @developer,
                        category = @category,
                        rating = @rating,
                        rating_count = @rating_count,
                        price = @price,
                        version = @version,
                        size_mb = @size_mb,
                        country = @country,
                        age_rating = @age_rating,
                        color = @color,
                        is_featured = @is_featured,
                        is_in_stock = @is_in_stock,
                        download_count = @download_count,
                        discount_percent = @discount_percent,
                        release_date = @release_date,
                        tags = @tags
                    where id = @id
                """;

            using var command = new NpgsqlCommand(sql, connection);
            FillAppParameters(command, app);
            command.ExecuteNonQuery();
        }

        public override void DeleteApp(Guid id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                using (
                    var deleteLinks = new NpgsqlCommand(
                        "delete from users_apps where app_id = @app_id",
                        connection,
                        transaction
                    )
                )
                {
                    deleteLinks.Parameters.AddWithValue("app_id", id);
                    deleteLinks.ExecuteNonQuery();
                }

                using (
                    var deleteApp = new NpgsqlCommand(
                        "delete from apps where id = @id",
                        connection,
                        transaction
                    )
                )
                {
                    deleteApp.Parameters.AddWithValue("id", id);
                    deleteApp.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public override void DownloadApp(Guid id)
        {
            if (_userId == Guid.Empty)
                return;

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                bool appExists;
                using (
                    var appExistsCmd = new NpgsqlCommand(
                        "select exists(select 1 from apps where id = @id)",
                        connection,
                        transaction
                    )
                )
                {
                    appExistsCmd.Parameters.AddWithValue("id", id);
                    appExists = (bool)appExistsCmd.ExecuteScalar()!;
                }

                if (!appExists)
                {
                    transaction.Rollback();
                    return;
                }

                bool recordExists;
                using (
                    var recordExistsCmd = new NpgsqlCommand(
                        "select exists(select 1 from users_apps where user_id = @user_id and app_id = @app_id)",
                        connection,
                        transaction
                    )
                )
                {
                    recordExistsCmd.Parameters.AddWithValue("user_id", _userId);
                    recordExistsCmd.Parameters.AddWithValue("app_id", id);
                    recordExists = (bool)recordExistsCmd.ExecuteScalar()!;
                }

                if (!recordExists)
                {
                    using var insertLink = new NpgsqlCommand(
                        """
                        insert into users_apps (user_id, app_id, installed_at)
                        values (@user_id, @app_id, @installed_at)
                        """,
                        connection,
                        transaction
                    );

                    insertLink.Parameters.AddWithValue("user_id", _userId);
                    insertLink.Parameters.AddWithValue("app_id", id);
                    insertLink.Parameters.AddWithValue("installed_at", DateTime.UtcNow);
                    insertLink.ExecuteNonQuery();

                    using var incDownloads = new NpgsqlCommand(
                        "update apps set download_count = download_count + 1 where id = @id",
                        connection,
                        transaction
                    );

                    incDownloads.Parameters.AddWithValue("id", id);
                    incDownloads.ExecuteNonQuery();
                }
                else
                {
                    using var updateLink = new NpgsqlCommand(
                        """
                        update users_apps
                        set installed_at = @installed_at
                        where user_id = @user_id and app_id = @app_id
                        """,
                        connection,
                        transaction
                    );

                    updateLink.Parameters.AddWithValue("user_id", _userId);
                    updateLink.Parameters.AddWithValue("app_id", id);
                    updateLink.Parameters.AddWithValue("installed_at", DateTime.UtcNow);
                    updateLink.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public override void UninstallApp(Guid id)
        {
            if (_userId == Guid.Empty)
                return;

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                int deletedRows;
                using (
                    var deleteLink = new NpgsqlCommand(
                        """
                        delete from users_apps
                        where user_id = @user_id and app_id = @app_id
                        """,
                        connection,
                        transaction
                    )
                )
                {
                    deleteLink.Parameters.AddWithValue("user_id", _userId);
                    deleteLink.Parameters.AddWithValue("app_id", id);
                    deletedRows = deleteLink.ExecuteNonQuery();
                }

                if (deletedRows > 0)
                {
                    using var decDownloads = new NpgsqlCommand(
                        """
                        update apps
                        set download_count = case
                            when download_count > 0 then download_count - 1
                            else 0
                        end
                        where id = @id
                        """,
                        connection,
                        transaction
                    );

                    decDownloads.Parameters.AddWithValue("id", id);
                    decDownloads.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public override void RestoreDefaults()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                using (
                    var clearLinks = new NpgsqlCommand(
                        "delete from users_apps",
                        connection,
                        transaction
                    )
                )
                    clearLinks.ExecuteNonQuery();

                using (
                    var clearApps = new NpgsqlCommand("delete from apps", connection, transaction)
                )
                    clearApps.ExecuteNonQuery();

                using (
                    var clearUsers = new NpgsqlCommand("delete from users", connection, transaction)
                )
                    clearUsers.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            SeedUsersToDb();
            SeedAppsToDb();
        }

        public override List<User> GetAllUsers()
        {
            var users = new List<User>();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            const string sql = """
                    select id, login, password_hash, first_name, last_name, email, role, avatar_color
                    from users
                    order by login
                """;

            using var command = new NpgsqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
                users.Add(MapUser(reader));

            return users;
        }

        public override User? GetUserByLogin(string login)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            const string sql = """
                    select id, login, password_hash, first_name, last_name, email, role, avatar_color
                    from users
                    where lower(login) = lower(@login)
                    limit 1
                """;

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("login", login);

            using var reader = command.ExecuteReader();
            if (!reader.Read())
                return null;

            return MapUser(reader);
        }

        public override void AddUser(User user)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            const string sql = """
                    insert into users
                    (
                        id, login, password_hash, first_name, last_name, email, role, avatar_color
                    )
                    values
                    (
                        @id, @login, @password_hash, @first_name, @last_name, @email, @role, @avatar_color
                    )
                """;

            using var command = new NpgsqlCommand(sql, connection);
            FillUserParameters(command, user);
            command.ExecuteNonQuery();
        }

        public override void UpdateUser(User user)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            const string sql = """
                    update users
                    set login = @login,
                        password_hash = @password_hash,
                        first_name = @first_name,
                        last_name = @last_name,
                        email = @email,
                        role = @role,
                        avatar_color = @avatar_color
                    where id = @id
                """;

            using var command = new NpgsqlCommand(sql, connection);
            FillUserParameters(command, user);
            command.ExecuteNonQuery();
        }

        private void EnsureDatabaseCreated()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            const string sql = """
                    create table if not exists apps
                    (
                        id uuid primary key,
                        short_name text not null,
                        full_name text not null,
                        description text not null,
                        developer text not null,
                        category text not null,
                        rating double precision not null,
                        rating_count integer not null,
                        price double precision not null,
                        version text not null,
                        size_mb double precision not null,
                        country text not null,
                        age_rating text not null,
                        color text not null,
                        is_featured boolean not null,
                        is_in_stock boolean not null,
                        download_count integer not null,
                        discount_percent double precision null,
                        release_date timestamp without time zone not null,
                        tags text not null
                    );

                    create table if not exists users
                    (
                        id uuid primary key,
                        login text not null unique,
                        password_hash text not null,
                        first_name text null,
                        last_name text null,
                        email text null,
                        role integer not null,
                        avatar_color text not null
                    );

                    create table if not exists users_apps
                    (
                        user_id uuid not null,
                        app_id uuid not null,
                        installed_at timestamp without time zone not null,
                        primary key (user_id, app_id),
                        constraint fk_users_apps_user
                            foreign key (user_id) references users(id) on delete cascade,
                        constraint fk_users_apps_app
                            foreign key (app_id) references apps(id) on delete cascade
                    );
                """;

            using var command = new NpgsqlCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        private void EnsureSeeded()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            bool hasApps;
            bool hasUsers;

            using (var appsCmd = new NpgsqlCommand("select exists(select 1 from apps)", connection))
                hasApps = (bool)appsCmd.ExecuteScalar()!;

            using (
                var usersCmd = new NpgsqlCommand("select exists(select 1 from users)", connection)
            )
                hasUsers = (bool)usersCmd.ExecuteScalar()!;

            if (!hasApps)
                SeedAppsToDb();

            if (!hasUsers)
                SeedUsersToDb();
        }

        private void SeedAppsToDb()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            foreach (var app in SeedApps())
            {
                using var command = new NpgsqlCommand(
                    """
                    insert into apps
                    (
                        id, short_name, full_name, description, developer, category,
                        rating, rating_count, price, version, size_mb, country,
                        age_rating, color, is_featured, is_in_stock, download_count,
                        discount_percent, release_date, tags
                    )
                    values
                    (
                        @id, @short_name, @full_name, @description, @developer, @category,
                        @rating, @rating_count, @price, @version, @size_mb, @country,
                        @age_rating, @color, @is_featured, @is_in_stock, @download_count,
                        @discount_percent, @release_date, @tags
                    )
                    """,
                    connection
                );

                FillAppParameters(command, app);
                command.ExecuteNonQuery();
            }
        }

        private void SeedUsersToDb()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            foreach (var user in SeedUsers())
            {
                using var command = new NpgsqlCommand(
                    """
                    insert into users
                    (
                        id, login, password_hash, first_name, last_name, email, role, avatar_color
                    )
                    values
                    (
                        @id, @login, @password_hash, @first_name, @last_name, @email, @role, @avatar_color
                    )
                    """,
                    connection
                );

                FillUserParameters(command, user);
                command.ExecuteNonQuery();
            }
        }

        private HashSet<Guid> GetInstalledAppIds()
        {
            var result = new HashSet<Guid>();

            if (_userId == Guid.Empty)
                return result;

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            const string sql = """
                    select app_id
                    from users_apps
                    where user_id = @user_id
                """;

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("user_id", _userId);

            using var reader = command.ExecuteReader();
            while (reader.Read())
                result.Add(reader.GetGuid(0));

            return result;
        }

        private bool IsAppDownloaded(Guid appId)
        {
            if (_userId == Guid.Empty)
                return false;

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            const string sql = """
                    select exists(
                        select 1
                        from users_apps
                        where user_id = @user_id and app_id = @app_id
                    )
                """;

            using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("user_id", _userId);
            command.Parameters.AddWithValue("app_id", appId);

            return (bool)command.ExecuteScalar()!;
        }

        private static App MapApp(NpgsqlDataReader reader)
        {
            return new App
            {
                Id = reader.GetGuid(0),
                ShortName = reader.GetString(1),
                FullName = reader.GetString(2),
                Description = reader.GetString(3),
                Developer = reader.GetString(4),
                Category = reader.GetString(5),
                Rating = reader.GetDouble(6),
                RatingCount = reader.GetInt32(7),
                Price = reader.GetDouble(8),
                Version = reader.GetString(9),
                SizeMB = reader.GetDouble(10),
                Country = reader.GetString(11),
                AgeRating = reader.GetString(12),
                Color = reader.GetString(13),
                IsFeatured = reader.GetBoolean(14),
                IsInStock = reader.GetBoolean(15),
                DownloadCount = reader.GetInt32(16),
                DiscountPercent = reader.IsDBNull(17) ? null : reader.GetDouble(17),
                ReleaseDate = reader.GetDateTime(18),
                Tags = ParseTags(reader.GetString(19)),
            };
        }

        private static User MapUser(NpgsqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetGuid(0),
                Login = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                FirstName = reader.IsDBNull(3) ? null : reader.GetString(3),
                LastName = reader.IsDBNull(4) ? null : reader.GetString(4),
                Email = reader.IsDBNull(5) ? null : reader.GetString(5),
                Role = (UserRole)reader.GetInt32(6),
                AvatarColor = reader.GetString(7),
            };
        }

        private static void FillAppParameters(NpgsqlCommand command, App app)
        {
            command.Parameters.AddWithValue("id", app.Id);
            command.Parameters.AddWithValue("short_name", app.ShortName);
            command.Parameters.AddWithValue("full_name", app.FullName);
            command.Parameters.AddWithValue("description", app.Description);
            command.Parameters.AddWithValue("developer", app.Developer);
            command.Parameters.AddWithValue("category", app.Category);
            command.Parameters.AddWithValue("rating", app.Rating);
            command.Parameters.AddWithValue("rating_count", app.RatingCount);
            command.Parameters.AddWithValue("price", app.Price);
            command.Parameters.AddWithValue("version", app.Version);
            command.Parameters.AddWithValue("size_mb", app.SizeMB);
            command.Parameters.AddWithValue("country", app.Country);
            command.Parameters.AddWithValue("age_rating", app.AgeRating);
            command.Parameters.AddWithValue("color", app.Color);
            command.Parameters.AddWithValue("is_featured", app.IsFeatured);
            command.Parameters.AddWithValue("is_in_stock", app.IsInStock);
            command.Parameters.AddWithValue("download_count", app.DownloadCount);
            command.Parameters.AddWithValue(
                "discount_percent",
                app.DiscountPercent.HasValue ? app.DiscountPercent.Value : DBNull.Value
            );
            command.Parameters.AddWithValue("release_date", app.ReleaseDate);
            command.Parameters.AddWithValue("tags", SerializeTags(app.Tags));
        }

        private static void FillUserParameters(NpgsqlCommand command, User user)
        {
            command.Parameters.AddWithValue("id", user.Id);
            command.Parameters.AddWithValue("login", user.Login);
            command.Parameters.AddWithValue("password_hash", user.PasswordHash);
            command.Parameters.AddWithValue("first_name", (object?)user.FirstName ?? DBNull.Value);
            command.Parameters.AddWithValue("last_name", (object?)user.LastName ?? DBNull.Value);
            command.Parameters.AddWithValue("email", (object?)user.Email ?? DBNull.Value);
            command.Parameters.AddWithValue("role", (int)user.Role);
            command.Parameters.AddWithValue("avatar_color", user.AvatarColor);
        }

        private static string SerializeTags(List<string> tags) =>
            tags == null || tags.Count == 0 ? string.Empty : string.Join(",", tags);

        private static List<string> ParseTags(string? raw) =>
            string.IsNullOrWhiteSpace(raw)
                ? new List<string>()
                : raw.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToList();
    }
}
