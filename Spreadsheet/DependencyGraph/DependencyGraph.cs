// Skeleton implementation written by Joe Zachary for CS 3500, January 2018.
// Filled in by Eric Naegle u0725372 on 1/31/2018

using System;
using System.Collections.Generic;

namespace Dependencies
{
    /// <summary>
    /// A DependencyGraph can be modeled as a set of dependencies, where a dependency is an ordered 
    /// pair of strings.  Two dependencies (s1,t1) and (s2,t2) are considered equal if and only if 
    /// s1 equals s2 and t1 equals t2.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that the dependency (s,t) is in DG 
    ///    is called the dependents of s, which we will denote as dependents(s).
    ///        
    ///    (2) If t is a string, the set of all strings s such that the dependency (s,t) is in DG 
    ///    is called the dependees of t, which we will denote as dependees(t).
    ///    
    /// The notations dependents(s) and dependees(s) are used in the specification of the methods of this class.
    ///
    /// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    ///     dependents("a") = {"b", "c"}
    ///     dependents("b") = {"d"}
    ///     dependents("c") = {}
    ///     dependents("d") = {"d"}
    ///     dependees("a") = {}
    ///     dependees("b") = {"a"}
    ///     dependees("c") = {"a"}
    ///     dependees("d") = {"b", "d"}
    ///     
    /// All of the methods below require their string parameters to be non-null.  This means that 
    /// the behavior of the method is undefined when a string parameter is null.  
    ///
    /// IMPORTANT IMPLEMENTATION NOTE
    /// 
    /// The simplest way to describe a DependencyGraph and its methods is as a set of dependencies, 
    /// as discussed above.
    /// 
    /// However, physically representing a DependencyGraph as, say, a set of ordered pairs will not
    /// yield an acceptably efficient representation.  DO NOT USE SUCH A REPRESENTATION.
    /// 
    /// You'll need to be more clever than that.  Design a representation that is both easy to work
    /// with as well acceptably efficient according to the guidelines in the PS3 writeup. Some of
    /// the test cases with which you will be graded will create massive DependencyGraphs.  If you
    /// build an inefficient DependencyGraph this week, you will be regretting it for the next month.
    /// </summary>
    public class DependencyGraph
    {
        //I use two HashMaps to keep track of the dependencies so that it's equally fast
        private Dictionary<string, HashSet<string> > dependeesList;
        private Dictionary<string, HashSet<string> > dependentsList;
        private int size;

        /// <summary>
        /// Creates a DependencyGraph containing no dependencies.
        /// </summary>
        public DependencyGraph()
        {
            this.dependeesList = new Dictionary<string,HashSet<string>>();
            this.dependentsList = new Dictionary<string, HashSet<string>>();
            this.size = 0;
        }

        /// <summary>
        /// This is the constructor that creates a dependency graph that contains the exact same values as the graph
        /// passed in, but they are separate objects. Throws ArgumentNullException if the parameter graph is null.
        /// </summary>
        /// <param name="graph"></param>
        public DependencyGraph(DependencyGraph graph)
        {
            if (graph == null)
                throw new ArgumentNullException();

            //create new dictionaries to make sure the copy is unique
            var newdependentsList = new Dictionary<string, HashSet<string>>();
            var newdependeesList = new Dictionary<string, HashSet<string>>();
            foreach (var item in graph.dependentsList)
            {
                //for each key, add a new HashSet that has original values
                newdependentsList.Add(item.Key,new HashSet<string>(item.Value));
            }

            //set this object's dictionaries to the ones I just created
            this.dependentsList = newdependentsList;

            foreach (var item in graph.dependeesList)
            {
                newdependeesList.Add(item.Key,new HashSet<string>(item.Value));
            }

            this.dependeesList = newdependeesList;

            //var dependents = new Dictionary<string, HashSet<string>>(graph.dependentsList);
            //var dependees = new Dictionary<string,HashSet<string>>(graph.dependeesList);
            //this.dependentsList = dependents;
            //this.dependeesList = dependees;
            this.size = graph.size;
        }

        /// <summary>
        /// The number of dependencies in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return size; }
        }

    /// <summary>
    /// Reports whether or not a string has dependents. If the parameter is null, an ArgumentNullException will be thrown.
    /// </summary>
    /// <param> string dependee </param>
    /// <returns></returns>
        public bool HasDependents(string s)
        {
            if (s == null)
                throw new ArgumentNullException("parameter cannot be null");

            //I remove the key from the dictionary if it has no dependents so I can just check if the key is
            //in the dictionary to know if it has dependents or dependees
            if (dependentsList.ContainsKey(s))
                return true;
            
            else return false;
        }

        /// <summary>
        /// Reports whether or nat a string has dependees. If the parameter is null, an ArgumentNullException will be thrown.
        /// </summary>
        /// <param> string dependent </param>
        /// <returns></returns>
        public bool HasDependees(string s)
        {
            if (s == null)
                throw new ArgumentNullException("parameter cannot be null");

            //I remove the key from the dictionary if it has no dependees so I can just check if the key is
            //in the dictionary to know if it has dependents or dependees
            if (dependeesList.ContainsKey(s))
                return true;

            else return false;
        }

        /// <summary>
        /// Enumerates dependents(s). Throws ArgumentNullException if the parameter is null.
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (s == null)
                throw new ArgumentNullException("parameter cannot be null");

            if (HasDependents(s))
            {
                dependentsList.TryGetValue(s, out HashSet<string> dependents);
                return dependents;
            }

            return new HashSet<string>();
        }

        /// <summary>
        /// Enumerates dependees(s). Throws ArgumentNullException if the parameter is null
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (s == null)
                throw new ArgumentNullException("parameter cannot be null");

            if (HasDependees(s))
            {
                dependeesList.TryGetValue(s, out HashSet<string> dependents);
                return dependents;
            }

            return new HashSet<string>();
        }

        /// <summary>
        /// Adds the dependency (s,t) to this DependencyGraph.
        /// This has no effect if (s,t) already belongs to this DependencyGraph.
        /// Throws ArgumentNullException if any of the parameters are null.
        /// </summary>
        public void AddDependency(string s, string t)
        {
            if (s == null || t == null)
                throw new ArgumentNullException("parameter cannot be null");

            //if s already has dependents
            if (dependentsList.TryGetValue(s, out HashSet<string> theDependents))
            {
                //add dependents and dependees to the current entries if it doesn't already exist
                if (!(theDependents.Contains(t)))
                {
                    size++;
                    theDependents.Add(t);

                    //if it has dependees, add a dependee
                    if (dependeesList.TryGetValue(t, out HashSet<string> theDependees))
                        theDependees.Add(s);

                    //otherwise, create new deependee list
                    else
                    {
                        var temp = new HashSet<string> { s };
                        dependeesList.Add(t, temp);
                    }
                }
            }

            else
            {
                //otherwise, create new dependent and dependee entries
                HashSet<string> dependents = new HashSet<string> { t };
                dependentsList.Add(s, dependents);

                //if it already has dependees, add
                if (dependeesList.TryGetValue(t, out HashSet<string> theDependees))
                {
                    theDependees.Add(s);
                    size++;
                }

                //otherwise make new dependees list
                else
                {
                    HashSet<string> dependees = new HashSet<string> { s };
                    dependeesList.Add(t, dependees);
                    size++;
                }
            }
        }

        /// <summary>
        /// Removes the dependency (s,t) from this DependencyGraph.
        /// Does nothing if (s,t) doesn't belong to this DependencyGraph.
        /// Throws ArgumentNullException if any of the parameters are null.
        /// </summary>
        public void RemoveDependency(string s, string t)
        {
            if (s == null || t == null)
                throw new ArgumentNullException("parameter cannot be null");

            if (dependentsList.TryGetValue(s, out HashSet<string> dependents))
            {
                //if the dependency exists, remove it
                if (dependents.Contains(t))
                {
                    dependents.Remove(t);
                        
                    //remove the key if it has no dependents
                    if (dependents.Count == 0)
                        dependentsList.Remove(s);

                    if (dependeesList.TryGetValue(t, out HashSet<string> dependees))
                    {
                        dependees.Remove(s);

                        //remove the key if it has no dependents
                        if (dependees.Count == 0)
                            dependeesList.Remove(t);
                    }

                    //reduce size once
                    size--;
                }
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (s,r).  Then, for each
        /// t in newDependents, adds the dependency (s,t).
        /// Throws ArgumentNullException if any of the parameters are null.
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (s == null || newDependents == null)
                throw new ArgumentNullException("parameter cannot be null");

            //if s has dependents
            if (dependentsList.TryGetValue(s, out HashSet<string> x))
            {
                //make a copy, loop through it, and delete the entire dependencies, not just the dependents
                HashSet<string> temp = new HashSet<string>(x);

                foreach (string t in temp)
                {
                    if (t != null)
                        RemoveDependency(s, t);
                }
            }

            //create new dependencies, not just adding dependents to s
            foreach (string t in newDependents)
            {
                if (t != null)
                    AddDependency(s, t);
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (r,t).  Then, for each 
        /// s in newDependees, adds the dependency (s,t).
        /// Throws ArgumentNullException if any of the parameters are null.
        /// </summary>
        public void ReplaceDependees(string t, IEnumerable<string> newDependees)
        {
            if (t == null || newDependees == null)
                throw new ArgumentNullException("parameter cannot be null");

            //if t has dependees
            if (dependeesList.TryGetValue(t, out HashSet<string> x))
            {
                //create a copy to loop through and delete the dependencies
                HashSet<string> temp = new HashSet<string>(x);

                foreach (string s in temp)
                {
                    if (s != null)
                        RemoveDependency(s,t);
                }
            }

            //then create new dependencies
            foreach (string s in newDependees)
            {
                if (s != null)
                    AddDependency(s,t);
            }
        }
    }
}
