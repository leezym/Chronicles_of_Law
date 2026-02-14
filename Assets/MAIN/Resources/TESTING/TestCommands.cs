using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using COMMANDS;

public class TestCommands : MonoBehaviour
{
    void Start()
    {        
        //StartCoroutine(Running());
    }

    /*private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
            CommandManager.Instance.Execute("moveCharDemo", "left");
        if(Input.GetKeyDown(KeyCode.RightArrow))
            CommandManager.Instance.Execute("moveCharDemo", "right");
    }*/

    IEnumerator Running()
    {
        yield return CommandManager.Instance.Execute("print");
        yield return CommandManager.Instance.Execute("print_1p", "Hello world");
        yield return CommandManager.Instance.Execute("print_mp", "Line1", "Line2", "Line3");

        yield return CommandManager.Instance.Execute("lambda");
        yield return CommandManager.Instance.Execute("lambda_1p", "Hello lambda");
        yield return CommandManager.Instance.Execute("lambda_mp", "Lambda1", "Lambda2", "Lambda3");

        yield return CommandManager.Instance.Execute("process");
        yield return CommandManager.Instance.Execute("process_1p", "3");
        yield return CommandManager.Instance.Execute("process_mp", "Process1", "Process2", "Process3");
    }
}
