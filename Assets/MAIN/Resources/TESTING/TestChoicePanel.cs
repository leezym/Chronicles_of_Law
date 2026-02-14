using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestChoicePanel : MonoBehaviour
{
    public ChoicePanel panel;
    // Start is called before the first frame update
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            panel = ChoicePanel.Instance;
            
            string[] choices = new string[]
            {
                "Cuando exista una unión marital de hecho por un lapso no inferior a dos años e impedimento legal para contraer matrimonio por parte de uno o de ambos compañeros permanentes, siempre y cuando la sociedad o sociedades conyugales anteriores hayan sido disueltas y liquidadas por lo menos un año antes de la fecha en que se inició la unión marital de hecho.",
                "Cuando exista unión marital de hecho durante un lapso no inferior a dos años, entre un hombre y una mujer sin impedimento legal para contraer matrimonio.",
                "Cuando exista unión marital de hecho durante un lapso no inferior a dos años, entre un hombre y una mujer sin impedimento legal para contraer matrimonio o; Cuando exista una unión marital de hecho por un lapso no inferior a dos años e impedimento legal para contraer matrimonio por parte de uno o de ambos compañeros permanentes, siempre y cuando la sociedad o sociedades conyugales anteriores hayan sido disueltas antes de la fecha en que se inició la unión marital de hecho."
            };

            panel.Show("¿Cuáles son los requisitos para presumir una UMH?", choices);
        }
    }
}
