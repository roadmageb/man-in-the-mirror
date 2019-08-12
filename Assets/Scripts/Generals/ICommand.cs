using UnityEngine;
using System.Collections;

public interface ICommand
{
    void Do();
    void Undo();
}
