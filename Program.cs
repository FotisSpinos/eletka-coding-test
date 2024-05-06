// The total number of direct and indirect orbits is 270768.

namespace Elekta;

using LocalOrbit = (string objectName, string orbitObjectName);

class CodingTest
{
    private class CelestialObject
    {
        public CelestialObject() { }

        public List<CelestialObject> OrbitObjects { set; get; } = new List<CelestialObject>();
    }

    private static void Main()
    {
        var tokens = ParseMapData();
        var objectsByName = new Dictionary<string, CelestialObject>();

        tokens.ForEach(t =>
        {
            if (!objectsByName.ContainsKey(t.objectName))
            {
                objectsByName.Add(t.objectName, new CelestialObject());
            }

            if (!objectsByName.ContainsKey(t.orbitObjectName))
            {
                objectsByName.Add(t.orbitObjectName, new CelestialObject());
            }

            var celestialObject = objectsByName[t.objectName];
            var orbitObject = objectsByName[t.orbitObjectName];

            celestialObject.OrbitObjects.Add(orbitObject);
        });

        const string ComName = "COM";
        var com = objectsByName[ComName];
        Console.WriteLine(OrbitCount(com));
    }

    private static int OrbitCount(CelestialObject com)
    {
        int OrbitCount(CelestialObject node, int depth)
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

