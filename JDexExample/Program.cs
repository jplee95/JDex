using System;
using JDex;

namespace JDexTest {
    class Program {
        static void Main(string[ ] args) {
            JDexNode node = null;
            using(var reader = new JDexReader("test.jdex")) {
                node = reader.Read( );

                Console.WriteLine(node.ToString( ));
                Console.WriteLine(reader.ReadToEnd( ));
            }

            JDexNode root = new JDexNode( );
            JDexNode nextNode = new JDexNode( );
            nextNode.AddValue("Something");
            root.Add("other", nextNode);

            root.Remove(nextNode);
            Console.WriteLine(nextNode.Key);
            root.Add("newKey", nextNode);
            Console.WriteLine(root);

            var nn = JDexNode.Parse("something: \"\\\" \\u0065\"");
            Console.WriteLine(nn["something", 0][0]);
            
            using(var writer = new JDexWriter("other.jdex")) {
                writer.Write(root);
                if(node != null) writer.Write(node);
            }

            using(var reader = new JDexReader("other.jdex")) {
                Console.WriteLine(reader.ReadToEnd( ));
            }
        }
    }
}
