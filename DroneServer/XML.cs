using System;
using System.Collections.Generic;
//using System.Text;
//using System.Net;
//using System.Net.Sockets;
//using System.IO;
//using System.Threading;
//using System.Collections;

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
		
		public XMLNode(string xmlstr) {
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

