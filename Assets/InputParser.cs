using System.Collections.Generic;

public static class InputParser {
    
    public static ParserNode[] Parse(string input, out float S2I, out float I2R, out float S2R, out int packetSize) {
        string[] lines = input.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.None);

        // Read node attributes
        int count = int.Parse(lines[0]);
        ParserNode[] nodes = new ParserNode[count];
        Dictionary<string, int> indices = new Dictionary<string, int>();
        string[] data;

        for (int i = 0; i < count; i++) {
            data = lines[i + 1].Split(' ');
            nodes[i] = new ParserNode(data[0], int.Parse(data[1]), int.Parse(data[2]));

            indices.Add(nodes[i].name, i);
        }

        // Read connections
        int connectionCount = int.Parse(lines[count + 1]);
        for (int i = count + 2; i < count + connectionCount + 2; i++) {
            data = lines[i].Split(' ');

            int firstIndex, secondIndex;
            indices.TryGetValue(data[0], out firstIndex);
            indices.TryGetValue(data[1], out secondIndex);

            nodes[firstIndex].connectedTo.Add(nodes[secondIndex]);
            nodes[secondIndex].connectedTo.Add(nodes[firstIndex]);
        }

        // assign the static data
        data = lines[lines.Length - 2].Split(' ');
        S2I = int.Parse(data[0]);
        I2R = int.Parse(data[1]);
        S2R = int.Parse(data[2]);

        packetSize = int.Parse(lines[lines.Length - 1]);

        return nodes;
    }
	
}

public struct ParserNode {
    public string name;

    public int hostCount;
    public int infectedCount;

    public List<ParserNode> connectedTo;

    public ParserNode(string name, int hostCount, int infectedCount) {
        this.name = name;
        this.hostCount = hostCount;
        this.infectedCount = infectedCount;

        connectedTo = new List<ParserNode>();
    }
}