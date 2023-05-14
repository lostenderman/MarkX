# MarkX

MarkX is a tool for converting the XML representation of the AST of parsed Markdown input generated by [CommonMark](https://commonmark.org/) to a test file used in the [Markdown package](https://github.com/Witiko/markdown).

It consists of a library - MarkXLibrary, that performs this conversion and a command-line tool - MarkXConsole, that uses this library.

## Installation

1. Clone this repository.
2. Install any missing third-party libraries (listed below).

### Prerequisites

- .NET 6.0

#### Third-party libraries

- [CommandLine library](https://github.com/commandlineparser/commandline)

## Quick example

1. Change the current working directory to `MarkXConsole`.
2. Create an XML file named `input.xml`.
3. Navigate to the online [CommonMark parser tool](https://spec.commonmark.org/dingus/).
4. Input any Markdown, such as `*a*`.
5. Navigate to the AST tab located above the window on the right.
6. Copy the generated XML in full.
7. Paste the XML into the file `input.xml`.
8. Run the command `dotnet run -- parse -f -I input.xml -O output.txt`.
9. See the output in the file `output.txt`.

## Usage

### Command-line interface

MarkX defines the following verbs and their options.

#### `parse` 
Parses input files and into an output file (JSON by default).

- `-I, --input`: defines input files and directories with top-level input files for parsing

- `-O, --output`: defines an output file or a directory name with the option `-t` enabled
- `-t, --tree`: parses the tests into a directory tree
- `-f, --file`: writes the result of parsing into a text file if the input is a single valid test
- `-e, --extensions`: enables listed extensions
- `-g, --group-sections`: groups tests into sections based on the name of the section to which they belong
- `-c, --code-indented`: overwrites the default rendering of the `code_block` from fenced code to an indented code
- `-i, --isolate-sections`: prevents identically named sections in different files from being merged
- `-m, --exclude-markdown`: excludes the Markdown input from the generated result

#### `check`
Checks input files against a provided result file.

- `-I, --input`: defines input files and directories with top-level input files for parsing

- `-R, --result`: specifies a file with a test result with which the tests are checked against
- `-e, --extensions`: enables listed extensions
- `-g, --group-sections`: groups tests into sections based on the name of the section to which they belong
- `-c, --code-indented`: overwrites the default rendering of the `code_block` from fenced code to an indented code
- `-r, --own-result`: makes the tests prioritise their expected result over the single provided one

Alternatively, the option `help` can be used to display information about MarkX or any of the verbs.

### File specification

#### Input

The MarkX input consists of one or more Input files or directories containing Input files. An Input file may either be in an XML or a JSON format.

1. XML - a single test

``` xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE document SYSTEM "CommonMark.dtd">

<document xmlns="http://commonmark.org/xml/1.0">
  <paragraph>
    <emph>
      <text>a</text>
    </emph>
  </paragraph>
</document>
```

2. JSON - a list of tests grouped by a section name, with additional properties

``` JSON
[
  {
    "name": "test",
    "tests": [
      {
        "markdown": "*a*\n",
        "xml": "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<!DOCTYPE document SYSTEM \"CommonMark.dtd\">\n\n<document xmlns=\"http://commonmark.org/xml/1.0\">\n\t<paragraph>\n\t\t<emph>\n\t\t\t<text>a</text>\n\t\t</emph>\n\t</paragraph>\n</document>",
        "example": 0
      }
    ]
  }
]
```

The only required property is `xml`.

#### Output

The output of MarkXConsole is only produced with the option `parse`, and it can take several forms:

1. Test result/file - only produced with a single valid test as an input

    - Test result

        ```
        documentBegin
        emphasis: a
        documentEnd
        ```

    - Test file

        ```
        <<<
        *a*
        >>>
        documentBegin
        emphasis: a
        documentEnd
        ```
       

2. JSON (default) - new property `output`
3. Directory tree - files grouped in section directories

### Examples

#### Parsing examples

- JSON to JSON

        parse -I <JSON_file> -O <JSON_file>

- JSON to a directory tree

        parse -t -I <JSON_file> -O <directory>

- directory of top-level XML files to a directory tree

        parse -t -I <directory> -O <directory>

- multiple test files with the `code_block` rendering overwritten and the tests grouped into sections

        parse -cg -I <file> <file> -O <JSON_OUTPUT>

#### Checking examples

- JSON with the results included

        check -I <JSON_file>

- XML file against a single result

        check -I <XML_file> -R <test_file>

- directory of top-level XML files against a single result

        parse -t -I <directory> -R <test_file>

- XML file against a single result with the `line_blocks` extension enabled

        check -e line_blocks -I <XML_file> -R <test_file>

