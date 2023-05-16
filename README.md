# MarkX

MarkX is a tool for converting the XML representation of the AST of parsed Markdown input generated by [CommonMark](https://commonmark.org/) to a test file used in the [Markdown package](https://github.com/Witiko/markdown).

It consists of a library - MarkXLibrary, that performs this conversion and a command-line tool - MarkXConsole, that uses this library.

## Installation

1. Clone this repository.
2. Install any missing third-party libraries (listed below).

### Prerequisites

- .NET 6.0

#### Third-party libraries

- [Command Line Parser Library][GHCL]

[GHCL]: https://github.com/commandlineparser/commandline

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

MarkX uses the [Command Line Parser Library][GHCL] to parse command-line arguments. Two verbs are defined, each with its own set of allowed options. However, both verbs also share a common set of options.

#### `parse` 
Parses the tests within the input files into the output file.

- `-O <output>, --output <output>`: defines an output file or a directory name with the option `-t` enabled

- `-t, --tree`: parses the tests into a directory tree
- `-f, --file`: writes the result of parsing into a single file if the input is a single valid test
- `-i, --full-index`: prefixes a name of an output file with the name of its section

#### `check`
Checks tests in input files against the provided result file.

- `-R <result>, --result <result>`: specifies a result file

- `-r, --own-result`: makes the tests prioritise their expected result over the single provided one

#### Shared options

- `-I <input> [<input>...], --input <input> [<input>...]`: defines input files and directories with top-level input files for parsing

- `-e [<extension>...], --extensions [<extension>...]`: enables listed extensions
- `-u, --ungroup-sections`: processes the tests as a single array of tests, ignoring the sections
- `-c, --code-indented`: overwrites the default `code_block` rendering from fenced code to an indented code
- `-s, --isolate-sections`: prevents identically named sections in different files from being merged
- `-m, --exclude-markdown`: excludes the Markdown input from the generated result
- `-q, --quiet`: suppresses printing results to console

### File specification

#### Input

The input consists of one or more input files or directories containing input files. An input file may either be in an XML or a JSON format.

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

2. JSON - an array of sections with tests

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

The output files are only produced with the option `parse`, and they can take several forms:

1. Test result/file

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

This section shows various examples of using the `parse` and `check` verbs. Input and output file arguments in angle brackets represent the paths to the respective files or directories.

- `parse`

    - JSON to JSON

            parse -I <JSON_file> -O <JSON_file>

    - JSON to a directory tree

            parse -t -I <JSON_file> -O <directory>

    - XML file with the `code_block` rendering overwritten

            parse -cf -I <XML_file> -O <test_file>

    - directory of top-level XML files to a directory tree

            parse -t -I <directory> -O <directory>

- `check`

    - JSON with results included
    
            check -I <JSON_file>
    
    - XML file against a single result
    
            check -I <XML_file> -R <test_file>
    
    - XML file against a single result with the `line_blocks` extension enabled

            check -e line_blocks -I <XML_file> -R <test_file>

    - directory of top-level XML files against a single result

            check -I <directory> -R <test_file>

