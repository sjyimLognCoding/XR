using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C : MonoBehaviour
{
    // Function vs Coroutine

    // Coroutine
    //    this allows you to spread the execution of a task over several frames.

    // Start is called before the first frame update
    void Start()
    {
        example_coroutine();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 3가지
    /*
    1. Creating a coroutine

    2. calling a coroutine
    we need to use a method StartCoroutine() provided by MonoBehaviour

    3. ending a coroutine
    */
    int i = 0;

    // i : 0 to 100 over 100 seconds / frames;

    public IEnumerator example_coroutine()
    {
        while (i < 100)
        {
            i++;
            // yield return new WaitForSeconds(1f);
        }

        print(i);

        yield break;
    }

    public IEnumerator yield1()
    {
        yield return new WaitForSeconds(1f);
        // yield return new WaitForEndOfFrame();
        yield return null;
        yield break;
    }

    // yield : it tells the thread what to do next
    // 1. temporarily deprioritise the current task, giving other tasks an opportunity to run
    // 2. also checks whether the current task is cancelled. (because in a tight CPU bound, certain jobs may not
    // check until the end)

}
