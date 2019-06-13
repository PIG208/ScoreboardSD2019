using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScoreboardSD2019
{
    public class Singer
    {
        public int ID { get; set; } = -1;
        public int Sequence { get; set; } = -1;
        public string Name { get; set; }
        public int GroupId { get; set; } = -1;
        public float[] Scores { get; set; } = new float[2];
        public Singer(Dictionary<string, object> element)
        {
            ID = (int)element?["id"];
            Sequence = (int)element?["sequence"];
            Name = (string)element["name"];
            GroupId = (int)(element["group_id"] ?? -1);
            Scores[0] = (float)(element?["score1"] ?? (float)0);
            Scores[1] = (float)(element?["score2"] ?? (float)0);
        }
    }
}
