using System.Collections.Generic;
using System.Collections.ObjectModel;

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

            // We already have one with this name
            if (indices.ContainsKey(data[0])) continue;

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

            int capacity = int.Parse(data[2]);
            nodes[firstIndex].AddConnectedNode(nodes[secondIndex], capacity);
            nodes[secondIndex].AddConnectedNode(nodes[firstIndex], capacity);
        }

        // assign the static data
        data = lines[lines.Length - 2].Split(' ');
        S2I = float.Parse(data[0].Replace('.', ','));
        I2R = float.Parse(data[1].Replace('.', ','));
        S2R = float.Parse(data[2].Replace('.', ','));

        packetSize = int.Parse(lines[lines.Length - 1]);

        return nodes;
    }
	
}

public struct ParserNode {
    public string name;

    public int hostCount;
    public int infectedCount;

    private List<ParserNodeConnection> connectedTo;
    public ReadOnlyCollection<ParserNodeConnection> ConnectedTo {
        get { return connectedTo.AsReadOnly(); }
    }

    public ParserNode(string name, int hostCount, int infectedCount) {
        this.name = name;
        this.hostCount = hostCount;
        this.infectedCount = infectedCount;

        connectedTo = new List<ParserNodeConnection>();
    }

    public void AddConnectedNode(ParserNode node, int capacity) {
        connectedTo.Add(new ParserNodeConnection(node, capacity));
    }

    public struct ParserNodeConnection {
        public ParserNode connectedTo;
        public int capacity;

        public ParserNodeConnection(ParserNode node, int capacity) {
            this.connectedTo = node;
            this.capacity = capacity;
        }
    }
}