using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;

namespace TESTING
{
    public class TestArchitect : MonoBehaviour
    {
        DialogueSystem ds;
        TextArchitect architect;

        public DialogueSystem.BuildMethod bm = DialogueSystem.BuildMethod.instant;

        string[] lines = new string[5]
        {
            "¡Hola, Martina!",
            "¡Hola, Ricardo! ¡Tanto tiempo! Qué casualidad encontrarnos en el supermercado, ¿no?",
            "Sí, realmente. ¿Cómo has estado todos estos años? Pensé que estabas viviendo fuera del país.",
            "¡Sí! Estuve muchos años viviendo en Francia, pero hace unos meses volví para quedarme. ¿Por qué no aprovechamos este encuentro y arreglamos para cenar algún día?",
            "Me encanta la idea. ¿El martes? ¿Qué dices?"
        };

        void Start()
        {
            ds = DialogueSystem.Instance;
            architect = new TextArchitect(ds.dialogueContainer.dialogueText);
            ds.buildMethod = DialogueSystem.BuildMethod.fade;
            architect.speed = 0.5f;
        }

        /*void Update()
        {
            if(bm != ds.buildMethod)
            {
                ds.buildMethod = bm;
                architect.Stop();
            }

            if(Input.GetKeyDown(KeyCode.S))
                architect.Stop();

            string longLine = "Los diálogos orales tienen lugar constantemente en la vida cotidiana y son nuestra principal forma de comunicación. Suelen ir acompañados de expresiones, gestos, ademanes, entonaciones, silencios, entre otros elementos, que complementan el mensaje verbal.";
            if(Input.GetKeyDown(KeyCode.Space))
            {
                if(architect.isBuilding)
                {
                    if(!architect.hurryUp)
                        architect.hurryUp = true;
                    else
                        architect.ForceComplete();
                }
                else
                    architect.Build(longLine);
            }
            else if(Input.GetKeyDown(KeyCode.A))
            {
                architect.Append(longLine);
            }
        }*/
    }
}
