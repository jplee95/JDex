# JDex C# (Might be a temperary name)

[![LICENSE](https://img.shields.io/github/license/jplee95/JDex)](LICENSE)
![STATUS](https://img.shields.io/github/actions/workflow/status/jplee95/JDex/dotnet.yml)

JDex is a simple indexing and value list file structure designed to be simply writen and lightweight.

# Features:

* Index valid name may include any of `a-Z`, `0-9`, `_`, and `.`
* Child index are assigned based on indent (`tab`) level
* Index can have a list of values of types: `string`, `bool`, `int`, and/or `float` sperated by commas (`,`)
* Index string values has escape characters and unicode support
* Comments are denoted by the pound (`#`) character

# File Formatting:

#### File_Example.jdex

<!-- NOTE: Borrowing toml syntax highlighting for comments -->
``` toml
# Comments can start on its own line
index_name: "string value", "second value"
    second_index: "value" # Or be at the end of an entry
        # Shorthand index declaration continue after the first index
        third_index: one: "one" two: "two"

numeric_values:
    int: -213768
    # Hex and binary values can not have a sign (+ or -)
    hex: 0x2222
    binary: 0b100100
    float: 0.002e-3

# allowed values for bool: true or false
boolean_values:
    bool: true
    bool: false

# You can have multiple index entries at root level
box_index: 2
    # Index keys can be repeated
    box: "big"
    box: "small"

# Unicode and other escape characters in value string
unicode_character: "\u0065"
exscape_character: "\n\t\f"
```

# C# Usage
In C# building JDex structure are done with `JDexNode`. The first node is the root node and will only contain other nodes. Writing and reading JDex files Can be done with `JDexWriter` or `JDexReader`.

``` c#
// First node is the root node, this only houses other nodes
var root = new JDex.JDexNode( );
var node = new JDex.JDexNode( );
root.Add("node_name", node);
node.AddValue("Node value");
node.AddValue(12);

// For writing files
using(var writer = new JDex.JDexWriter("file.jdex"))
    writer.Write(root);

// For reading files
using(var reader = new JDex.JDexReader("file.jdex")) {
    var node = reader.ReadToEnd( );
    System.Console.WriteLine(node);
}
```

Parsing of a string is posible through the use of `JDexNode.Parse(string)`.

``` c#
// Throws an JDexParseException if the structure was invalid
var node = JDexNode.Parse("first.node: \"Node Value\" inner.node: \"Inner Node Value\"");
```

Utility functions for traversing through and testing for node are `JDexNode.PathThrough(JDexNode, params string[])` and `JDexNode.HasPath(JDexNode, params string[])`. JDex paths are formed by chaining JDex key names seperated by a collin (`:`) for the path. The use of a pound (`#`) character after a JDex key stats the sub index for the key name.

``` c#
// Returns the node at the end of the path
var node = JDexNode.PathThrough(root, "node:node#4:node");
// Checks if the path is valid
var exists = JDexNode.HasPath(root, "node:node#4:node");
```
