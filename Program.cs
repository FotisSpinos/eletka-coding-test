// The total number of direct and indirect orbits is 270768.

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

    private static void Main()
    {
        var tokens = ParseMapData();
        var objectsByName = new Dictionary<string, CelestialObject>();

        tokens.ForEach(token =>
        {
            if (!objectsByName.TryGetValue(token.objectName, out CelestialObject? celestialObject))
            {
                celestialObject = new CelestialObject();
                objectsByName.Add(token.objectName, celestialObject);
            }

            if (!objectsByName.TryGetValue(token.orbitObjectName, out CelestialObject? orbitObject))
            {
                orbitObject = new CelestialObject();
                objectsByName.Add(token.orbitObjectName, orbitObject);
            }

            celestialObject.OrbitObjects.Add(orbitObject);
        });

        const string ComName = "COM";
        var com = objectsByName[ComName];
        Console.WriteLine(OrbitCount(com));
    }

    private static int OrbitCount([NotNull] CelestialObject com)
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

    [return: NotNull]
    private static List<LocalOrbit> ParseMapData()
    {
        const char Separator = ')';

        var orbits = new List<LocalOrbit>();

        var lines = File.ReadAllLines(Path.Combine(".", "data.txt"));
        lines.ToList().ForEach(line =>
        {
            var tokens = line.Split(Separator);
            orbits.Add((tokens[0], tokens[1]));
        });

        return orbits;
    }
}

