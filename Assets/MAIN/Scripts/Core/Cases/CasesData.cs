using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

namespace GAME
{
    [System.Serializable]
    public class CasesData
    {
        public enum CaseLevel {facil, intermedio, dificil}
        public CaseLevel level;
        public TextAsset fileToRead;
        public Sprite photo;
        public string name;
        public int age;
        public enum CaseArea {civil, familia, laboral, publico}
        public CaseArea area;
        public string description;
        public string abstracts;

        public CasesData(){}

        public CasesData(CaseLevel level, TextAsset fileToRead, Sprite photo, string name, int age, CaseArea area, string description, string abstracts)
        {
            this.level = level;
            this.photo = photo;
            this.name = name;
            this.age = age;
            this.area = area;
            this.description = description;
            this.abstracts = abstracts;
        }

        public static List<CasesData> Capture()
        {
            List<CasesData> list = new List<CasesData>();
            var cm = CasesManager.Instance;
            list = cm.casesData;
            
            return list;
        }

        public static void Apply(List<CasesData> data)
        {
            var cm = CasesManager.Instance;
            cm.casesData = data;
        }
    }
}