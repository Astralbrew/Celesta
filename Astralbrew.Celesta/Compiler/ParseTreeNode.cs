﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Astralbrew.Celesta.Compiler
{    
    public class ParseTreeNode
    {
        public string Label { get; set; }

        public string ScopeName { get; set; } = "";

        public ParseTreeNode[] Children { get; set; }

        public ParseTreeNode(string label, ParseTreeNode[] children)
        {
            Label = label;
            Children = children;
        }

        public ParseTreeNode Clone()
        {
            return new ParseTreeNode(Label, Children.ToArray());
        }

        public ParseTreeNode(string label) : this(label, new ParseTreeNode[0]) { }
        public bool IsTerminal => Children.Length == 0;        
        public override string ToString()
        {
            string[] chstr = Children.Select(x => x.ToString())
                .Select(s => string.Join("\n", s.Split('\n').Where(x => x != "").Select(x => "  " + x)))
                .ToArray();

            return $"{Label}{(Label == "~scope" ? ScopeName : "")}\n{string.Join("\n", chstr)}";
        }

        public ParseTreeNode Flatten()
        {            
            if (Children.Length == 2 && Children[1].Label == "~list") 
            {
                var first = Children[0].Flatten();
                var second = Children[1].Flatten();
                return new ParseTreeNode(Label, new ParseTreeNode[] { first }.Concat(second.Children).ToArray()) { ScopeName = ScopeName };
            }
            return new ParseTreeNode(Label, Children.Select(x => x.Flatten()).ToArray()) { ScopeName = ScopeName };

        }

        public void FixScopes(string currentScope = "")
        {
            //if(Label=="~scope")
            {
                ScopeName = currentScope + ScopeName;
                currentScope = ScopeName;
            }
            foreach (var c in Children)
                c.FixScopes(currentScope);
        }
    }    
}
