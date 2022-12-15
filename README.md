# JDex C# (Might be a temperary name)

[![LICENSE](https://img.shields.io/github/license/jplee95/JDex)](LICENSE)

JDex is a simple indexing and string value list file structure designed to be simple and lightweight.

# Features:
* Index valid name include all of '`a-Z`', '`0-9`', '`_`', and '`.`' if not on key bounds
* Child index are assigned based on indent (`tab`) level
* Index can have a list of quoted (`"`) string values using commas (`,`) for next value
* Index values have escape characters and unicode support
* Comments are denoted by the pound (`#`) character

# File Formatting:
#### File_Example.jdex
``` py
# Comments can start on its own line
index_name: "string value", "second value"
    second_index: "value" # Or be at the end of an entry
        # Shorthand index declaration continue after the first index
        third_index: one: "one" two: "two"

# You can have multiple index entries at root level
box_index: "2"
    # Index keys can be repeated
    box: "big"
    box: "small"
    
# Unicode and other escape characters in value string
unicode_index: "\u0065"
```

# C# Usage
In C# building JDex structure are done with `JDexNode`. The first node is the root node and will only contain other nodes. Writing and reading JDex files Can be done with `JDexWriter` or `JDexReader`.

``` c#
// First node is the root node, this only houses other nodes
var root = new JDex.JDexNode( );
var node = new JDex.JDexNode( );
root.Add("node_name", node);
node.AddValue("Node value");

// For writing files
using(var writer = new JDex.JDexWriter("file.jdex"))
    writer.Write(root);

// For reading files
using(var reader = new JDex.JDexReader("file.jdex")) {
    var node = reader.Read( );
    System.Console.WriteLine(node);
}
```

Parsing of string is posible as through the use of `JDexNode.Parse(string)`.
``` c#
var node = JDexNode.Parse("first.node: \"Node Value\" inner.node: \"Inner Node Value\"");
```

Utility functions for traversing through and testing for node are `JDexNode.PathThrough(JDexNode, params string[])` and `JDexNode.HasPath(JDexNode, params string[])`. JDex string paths are formed by chaining JDex key names seperated by a collin (`:`) for the path. The use of a pound (`#`) character after a JDex key stats the sub index for the key name.

``` c#
var node = JDexNode.PathThrough(root, "node:node#4:node");
```
