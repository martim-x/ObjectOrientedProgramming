#!/usr/bin/env python3
"""
Extracts classes, interfaces, properties, fields, methods, enums
from a C# project and prints a structured summary for UML diagram building.
"""

import os
import re
import sys

# ── helpers ──────────────────────────────────────────────────────────────────

ACCESS = r"(public|private|protected|internal|protected internal|private protected)"
TYPE = r"[\w\[\]<>,\s\?\.]+"


def find_cs_files(root):
    for dirpath, _, files in os.walk(root):
        for f in files:
            if f.endswith(".cs"):
                yield os.path.join(dirpath, f)


def strip_comments(src):
    src = re.sub(r"//.*", "", src)
    src = re.sub(r"/\*.*?\*/", "", src, flags=re.DOTALL)
    return src


# ── extractors ───────────────────────────────────────────────────────────────


def extract_types(src):
    """Return list of (kind, name, bases) for class/interface/enum/struct."""
    pattern = re.compile(
        r"(?:" + ACCESS + r"\s+)?"
        r"(?:abstract\s+|sealed\s+|static\s+|partial\s+)*"
        r"(class|interface|enum|struct)\s+"
        r"(\w+)"
        r"(?:\s*<[^>]+>)?"  # generics
        r"(?:\s*:\s*([\w,\s<>]+?))?"  # inheritance
        r"\s*[{]",
        re.MULTILINE,
    )
    results = []
    for m in pattern.finditer(src):
        kind = m.group(2)
        name = m.group(3)
        bases_raw = m.group(4) or ""
        bases = [b.strip() for b in bases_raw.split(",") if b.strip()]
        results.append((kind, name, bases))
    return results


def extract_fields(src):
    pattern = re.compile(
        r"^\s*(" + ACCESS + r")\s+"
        r"(?:static\s+|readonly\s+|const\s+)*"
        r"(" + TYPE + r")\s+"
        r"(_?\w+)\s*(?:=|;)",
        re.MULTILINE,
    )
    return [
        (m.group(1), m.group(3).strip(), m.group(4).strip())
        for m in pattern.finditer(src)
    ]


def extract_properties(src):
    pattern = re.compile(
        r"^\s*(" + ACCESS + r")\s+"
        r"(?:static\s+|virtual\s+|override\s+|abstract\s+|new\s+)*"
        r"(" + TYPE + r")\s+"
        r"([A-Z]\w*)\s*\{[^}]*(?:get|set)",
        re.MULTILINE,
    )
    return [
        (m.group(1), m.group(3).strip(), m.group(4).strip())
        for m in pattern.finditer(src)
    ]


def extract_methods(src):
    pattern = re.compile(
        r"^\s*(" + ACCESS + r")\s+"
        r"(?:static\s+|virtual\s+|override\s+|abstract\s+|async\s+|new\s+)*"
        r"(" + TYPE + r")\s+"
        r"(\w+)\s*"
        r"\(([^)]*)\)\s*(?:\{|=>|;)",
        re.MULTILINE,
    )
    results = []
    for m in pattern.finditer(src):
        access = m.group(1)
        return_type = m.group(3).strip()
        name = m.group(4).strip()
        params_raw = m.group(5).strip()
        # skip constructors caught as methods (they have no return type keyword but regex grabs them anyway)
        if name in ("if", "for", "while", "foreach", "switch", "using", "return"):
            continue
        # simplify params: keep only types
        params = []
        for p in params_raw.split(","):
            p = p.strip()
            if p:
                parts = p.split()
                params.append(parts[-2] if len(parts) >= 2 else p)
        results.append((access, return_type, name, params))
    return results


def extract_enums(src, name):
    pattern = re.compile(r"enum\s+" + re.escape(name) + r"\s*\{([^}]+)\}", re.DOTALL)
    m = pattern.search(src)
    if m:
        values = [
            v.strip().split("=")[0].strip() for v in m.group(1).split(",") if v.strip()
        ]
        return [v for v in values if v]
    return []


# ── main ─────────────────────────────────────────────────────────────────────


def analyse_file(path, root):
    with open(path, encoding="utf-8", errors="ignore") as f:
        raw = f.read()
    src = strip_comments(raw)
    rel = os.path.relpath(path, root)

    types = extract_types(src)
    if not types:
        return

    print(f"\n{'='*70}")
    print(f"FILE: {rel}")
    print("=" * 70)

    for kind, name, bases in types:
        print(f"\n  [{kind.upper()}] {name}", end="")
        if bases:
            print(f"  →  {', '.join(bases)}", end="")
        print()

        if kind == "enum":
            vals = extract_enums(src, name)
            for v in vals:
                print(f"      VALUE  {v}")
            continue

        # fields
        for acc, typ, fname in extract_fields(src):
            sym = "+" if acc == "public" else ("-" if acc == "private" else "#")
            print(f"      FIELD  {sym} {fname} : {typ}")

        # properties
        for acc, typ, pname in extract_properties(src):
            sym = "+" if acc == "public" else ("-" if acc == "private" else "#")
            print(f"      PROP   {sym} {pname} : {typ}")

        # methods
        for acc, ret, mname, params in extract_methods(src):
            sym = "+" if acc == "public" else ("-" if acc == "private" else "#")
            pstr = ", ".join(params) if params else ""
            print(f"      METHOD {sym} {mname}({pstr}) : {ret}")


def main():
    root = sys.argv[1] if len(sys.argv) > 1 else "."
    root = os.path.abspath(root)
    print(f"Scanning: {root}\n")

    files = sorted(find_cs_files(root))
    if not files:
        print("No .cs files found.")
        return

    for path in files:
        analyse_file(path, root)

    print(f"\n\n{'='*70}")
    print(f"Done. Scanned {len(files)} .cs file(s).")


if __name__ == "__main__":
    main()
