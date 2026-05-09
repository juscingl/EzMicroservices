import re
from pathlib import Path


ROOT = Path("D:/eztrade/Work/src")
TARGET_PREFIX = "BuildingBlocks"
SKIP_SUFFIXES = (
    ".g.cs",
    ".AssemblyInfo.cs",
    ".Designer.cs",
    "ModelSnapshot.cs",
)


TYPE_PATTERN = re.compile(
    r"^\s*public\s+(?:abstract\s+|sealed\s+|static\s+|partial\s+|readonly\s+|ref\s+)*"
    r"(class|interface|struct|record|enum|delegate)\s+([A-Za-z_][A-Za-z0-9_]*)"
)
METHOD_PATTERN = re.compile(
    r"^\s*public\s+(?:async\s+)?(?:override\s+|virtual\s+|static\s+|sealed\s+|partial\s+|unsafe\s+|new\s+)*"
    r"(?!class\b|interface\b|struct\b|record\b|enum\b|delegate\b)"
    r"[A-Za-z_][A-Za-z0-9_<>\[\]\?,\.\s]*\s+([A-Za-z_][A-Za-z0-9_]*)\s*\("
)
CTOR_PATTERN = re.compile(r"^\s*public\s+([A-Za-z_][A-Za-z0-9_]*)\s*\(")
PROPERTY_PATTERN = re.compile(
    r"^\s*public\s+(?:override\s+|virtual\s+|static\s+|sealed\s+|new\s+)*"
    r"[A-Za-z_][A-Za-z0-9_<>\[\]\?,\.\s]*\s+([A-Za-z_][A-Za-z0-9_]*)\s*(?:\{|=>)"
)
FIELD_PATTERN = re.compile(
    r"^\s*public\s+(?:const|static readonly|readonly|static)\s+"
    r"[A-Za-z_][A-Za-z0-9_<>\[\]\?,\.\s]*\s+([A-Za-z_][A-Za-z0-9_]*)\s*(?:=|;)"
)
INTERFACE_METHOD_PATTERN = re.compile(
    r"^\s*[A-Za-z_][A-Za-z0-9_<>\[\]\?,\.\s]*\s+[A-Za-z_][A-Za-z0-9_]*\s*\([^;]*\)\s*;"
)
INTERFACE_PROPERTY_PATTERN = re.compile(
    r"^\s*[A-Za-z_][A-Za-z0-9_<>\[\]\?,\.\s]*\s+[A-Za-z_][A-Za-z0-9_]*\s*\{\s*(?:get;\s*|set;\s*)+}"
)


def has_existing_xml(lines: list[str], index: int) -> bool:
    cursor = index - 1
    while cursor >= 0 and lines[cursor].strip() == "":
        cursor -= 1
    return cursor >= 0 and lines[cursor].lstrip().startswith("///")


def build_summary(line: str) -> str | None:
    type_match = TYPE_PATTERN.match(line)
    if type_match:
        name = type_match.group(2)
        return f"表示{name}。"

    ctor_match = CTOR_PATTERN.match(line)
    if ctor_match:
        name = ctor_match.group(1)
        return f"初始化{name}的新实例。"

    method_match = METHOD_PATTERN.match(line)
    if method_match:
        name = method_match.group(1)
        if name in {"if", "for", "while", "switch", "catch", "using", "lock", "return"}:
            return None
        return f"执行{name}。"

    property_match = PROPERTY_PATTERN.match(line)
    if property_match:
        name = property_match.group(1)
        return f"获取或设置{name}。"

    field_match = FIELD_PATTERN.match(line)
    if field_match:
        name = field_match.group(1)
        return f"表示{name}。"

    return None


def process_file(path: Path) -> bool:
    original = path.read_text(encoding="utf-8")
    lines = original.splitlines()
    changed = False
    final: list[str] = []
    interface_depth = 0
    brace_depth = 0

    for index, line in enumerate(lines):
        current_is_interface = interface_depth > 0
        summary = build_summary(line)
        if summary and not has_existing_xml(lines, index):
            indent = line[: len(line) - len(line.lstrip())]
            final.append(f"{indent}/// <summary>")
            final.append(f"{indent}/// {summary}")
            final.append(f"{indent}/// </summary>")
            changed = True
        elif current_is_interface and not has_existing_xml(lines, index):
            member_summary = None
            stripped = line.strip()
            if stripped and not stripped.startswith("}") and not stripped.startswith("{"):
                if INTERFACE_METHOD_PATTERN.match(line):
                    method_name_match = re.search(r"([A-Za-z_][A-Za-z0-9_]*)\s*\(", line)
                    if method_name_match:
                        member_summary = f"执行{method_name_match.group(1)}。"
                elif INTERFACE_PROPERTY_PATTERN.match(line):
                    property_name_match = re.search(r"\s([A-Za-z_][A-Za-z0-9_]*)\s*\{", line)
                    if property_name_match:
                        member_summary = f"获取或设置{property_name_match.group(1)}。"

            if member_summary:
                indent = line[: len(line) - len(line.lstrip())]
                final.append(f"{indent}/// <summary>")
                final.append(f"{indent}/// {member_summary}")
                final.append(f"{indent}/// </summary>")
                changed = True

        final.append(line)

        open_braces = line.count("{")
        close_braces = line.count("}")
        type_match = TYPE_PATTERN.match(line)
        if type_match and type_match.group(1) == "interface":
            interface_depth = brace_depth + open_braces - close_braces + 1
        brace_depth += open_braces - close_braces
        if interface_depth > 0 and brace_depth < interface_depth:
            interface_depth = 0

    if not changed:
        return False

    path.write_text("\n".join(final) + "\n", encoding="utf-8")
    return True


def is_target(path: Path) -> bool:
    if not path.name.endswith(".cs"):
        return False
    if path.name.endswith(SKIP_SUFFIXES):
        return False
    lower_parts = [part.lower() for part in path.parts]
    if "obj" in lower_parts or "bin" in lower_parts:
        return False
    relative = path.relative_to(ROOT)
    if not relative.parts:
        return False
    return relative.parts[0].startswith(TARGET_PREFIX)


def main() -> None:
    changed_files: list[str] = []
    for path in ROOT.rglob("*.cs"):
        if not is_target(path):
            continue
        if process_file(path):
            changed_files.append(str(path).replace("\\", "/"))

    print(f"changed={len(changed_files)}")
    for file_path in changed_files:
        print(file_path)


if __name__ == "__main__":
    main()
