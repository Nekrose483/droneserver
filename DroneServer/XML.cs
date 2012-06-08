using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace DroneServer
{
	public class XMLNode
	{
		public XMLNode parent;
		public List<XMLNode> childNodes = new List<XMLNode> ();
		public string tag;
		public string value;
		
		//XML creater
		public XMLNode (XMLNode parent_, string tag_, string value_)
		{
			parent = parent_;
			tag = tag_;
			value = value_;
		}	
		
		public void parseXML (string xmlstr) //does this do anything?
		{
			//Used to create data structure given a string
			//1. search from left to right for <[tag]>
			//2. search from right to left for </[tag]>
			//  if not found, return null 
			//		[indicates that the calling function should consider the string a value]
			//  if found,
			//		a. set tag to [tag]
			//		b. does value contain at least one [tag]?
			//			if not, set value to [value]
			//			if, 
			
			
			//convert string to stream
			byte[] byteArray = Encoding.UTF8.GetBytes (xmlstr);
			MemoryStream stream = new MemoryStream (byteArray);
			
			XPathDocument xmldoc = new XPathDocument (stream);
			XPathNavigator nav = xmldoc.CreateNavigator ();
			nav.MoveToRoot ();
			
			Console.Write ("The XML string for this PARENT ");
			Console.Write ("  " + nav.Name + " ");
			Console.WriteLine ("is '{0}'", nav.Value);
			
			
			nav.MoveToFirstChild ();
			
			
			
			do {
				//this code works, but it seems to only comb through 1 dept level
				//Find the first element.
				
				if (nav.NodeType == XPathNodeType.Element) {
					//if children exist
					if (nav.HasChildren == true) {

						//Move to the first child.
						nav.MoveToFirstChild ();

						//Loop through all the children.
						do {
							//Display the data.
							Console.Write ("The XML string for this child ");
							Console.Write ("  " + nav.Name + " ");
							Console.WriteLine ("is '{0}'", nav.Value);
							//Console.Write (" namespace: '{0}'", nav.GetNamespace);
							//Console.WriteLine (" OuterXml: " + nav.);
							
							//.OuterXml really seems to show innerxml

							//Check for attributes.
							if (nav.HasAttributes == true) {
								Console.WriteLine ("This node has attributes");
							}
						} while (nav.MoveToNext()); 
					} else {
						Console.Write ("The XML string for this PARENT ");
						Console.Write ("  " + nav.Name + " ");
						Console.WriteLine ("is '{0}'", nav.Value);

					}
				}
			} while (nav.MoveToNext());
		}
		
		public XMLNode lastChild ()
		{
			if (childNodes.Count > 0) {
				return childNodes [childNodes.Count - 1];
			} else {
				return null;
			}
				
		}
		
		public string makeXMLString ()
		{
			//call this on the root node and it'll interate through the children
			//recursively and return an XML formatted string
			
			string ret = "<" + tag + ">";
			
			if (childNodes.Count > 0) {
				foreach (XMLNode node in childNodes) {
					ret += node.makeXMLString ();
				}
			} else {
				ret += value;
			}
			ret += "</"+tag+">";
			
			return ret;
		}
	}
}

