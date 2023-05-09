# MarkX

MarkX is a tool for converting the XML representation of the AST of parsed Markdown input generated by [CommonMark](https://commonmark.org/) to a test file used in the [Markdown package](https://github.com/Witiko/markdown).

## Installation

1. Clone this repository.
2. Install any missing third-party libraries (listed below).

### Prerequisites

- .NET 6.0

#### Third-party libraries

- [CommandLine library](https://github.com/commandlineparser/commandline)

## Usage/Examples

1. Change the current working directory to `MarkXConsole`.
2. Create an XML file named `input.xml`.
3. Navigate to the online [CommonMark parser tool](https://spec.commonmark.org/dingus/).
4. Input any Markdown, such as `*a*`.
5. Navigate to the AST tab located above the window on the right.
6. Copy the generated XML in full.
7. Paste the XML into the file `input.xml`.
8. Run the command `dotnet run -- parse -f -I input.xml -O output.txt`.
9. See the output in the file `output.txt`.
