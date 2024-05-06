// Answer: The total number of direct and indirect orbits is 270768.

// Note: This script contains comments that are meant to communicate some of my thinking process to the reviewers at Elekta.
// The comments do not reflect production work that I would deliver.

namespace Elekta;

using System;
using System.Diagnostics.CodeAnalysis;
using LocalOrbit = (string objectName, string orbitObjectName);

public class CodingTest
{
    private class CelestialObject
    {
        public CelestialObject() { }

        [NotNull]
        public List<CelestialObject> OrbitObjects { set; get; } = [];
    }

    private static void Main(string[] args)
    {
        var dataFilePath = string.Empty;
        if (args.Length == 0)
        {
            const string DefaultDataFileName = "data.txt";
            dataFilePath = Path.Combine("..", "..", "..", DefaultDataFileName);
        }
        else if(args.Length == 1)
        {
            dataFilePath = args[0];
        }
        else
        {
            Console.Write($"Invalid number of arguments: received {args.Length}, expected 1 or no arguments.");
            Environment.Exit(0);
        }

        var tokens = ParseMapData(dataFilePath);
        var objectsByName = new Dictionary<string, CelestialObject>();

        tokens.ForEach(token =>
        {
            // It would be possible to create a helper method that either retrieves the celestial object from the dictionary
            // or creates and adds it to the dictionary. This could reduce the duplicate code below.
            if (!objectsByName.TryGetValue(token.objectName, out var celestialObject))
            {
                celestialObject = new CelestialObject();
                objectsByName.Add(token.objectName, celestialObject);
            }

            if (!objectsByName.TryGetValue(token.orbitObjectName, out var orbitObject))
            {
                orbitObject = new CelestialObject();
                objectsByName.Add(token.orbitObjectName, orbitObject);
            }

            celestialObject.OrbitObjects.Add(orbitObject);
        });

        const string ComName = "COM";
        if (!objectsByName.TryGetValue(ComName, out var com))
        {
            Console.Write("Could not find COM.");
            Environment.Exit(0);
        }
        Console.WriteLine($"Number of orbits: {OrbitCountRecursive(com)}.");
    }

    /// <summary>
    /// This method uses recursion to find the number of orbits. This is an elegant and simplistic solution
    /// but when the number of orbits is great, the program could crash with a stack overflow exception.
    /// </summary>
    private static int OrbitCountRecursive([NotNull] CelestialObject com)
    {
        static int OrbitCount([NotNull] CelestialObject node, int depth)
        {
            var output = depth;

            foreach (var child in node.OrbitObjects)
            {
                output += OrbitCount(child, depth + 1);
            }

            return output;
        }

        return OrbitCount(com, 0);
    }

    /// <summary>
    /// An alternative implementation of OrbitCountRecursive. This implementation is slightly more complected but it avoids the
    /// danger of failing due to a stack overflow exception even when the text file contains a lot of data.
    /// </summary>
    private static int OrbitCount([NotNull] CelestialObject com)
    {
        var queue = new Queue<CelestialObject?>();
        queue.Enqueue(com);
        queue.Enqueue(null);
        var depth = 0;
        var output = 0;

        while(queue.Any())
        {
            var current = queue.Dequeue();
            if (current is null)
            {
                queue.Enqueue(null);
                if (queue.Peek() is null)
                {
                    break;
                }
                depth++;
                continue;
            }

            current.OrbitObjects.ForEach(queue.Enqueue);
            output += depth;
        }

        return output;
    }

    [return: NotNull]
    private static List<LocalOrbit> ParseMapData([NotNull] string fullFilePath)
    {
        const char Separator = ')';
        var orbits = new List<LocalOrbit>();

        try
        {
            var lines = File.ReadAllLines(fullFilePath);
            lines.ToList().ForEach(line =>
            {
                var tokens = line.Split(Separator);

                if (tokens.Length != 2)
                {
                    // We could create a custom Exception that we could throw when parsing errors occur.
                    throw new Exception($"Invalid local orbit: '{line}'.");
                }

                orbits.Add((tokens[0], tokens[1]));
            });
        }
        catch(Exception e)
        {
            // For simplicity the method logs an exception and exists the application but there are many ways to handle this.
            // One would be to follow a data driven approach and let client code handle any occurring errors.
            Console.Write(e);
            Environment.Exit(0);
        }

        return orbits;
    }
}

