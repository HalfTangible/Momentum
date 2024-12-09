using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MomentumQueue : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


/*I asked ChatGPT for help with arranging the order of the sheets

 using System.Collections.Generic;
using System.Linq;

public class TurnQueue
{
    private List<StatSheet> queue = new List<StatSheet>();

    // Initialize the queue with a list of StatSheets, sorted by Momentum descending.
    public TurnQueue(List<StatSheet> sheets)
    {
        queue = sheets.OrderByDescending(sheet => sheet.getMomentumCurrent()).ToList();
    }

    // Gets the next StatSheet (highest Momentum).
    public StatSheet GetNextTurn()
    {
        if (queue.Count == 0)
            return null;

        return queue[0]; // First item in the list has the highest Momentum.
    }

    // Process the turn for the current StatSheet and reinsert it based on new Momentum.
    public void ProcessTurn()
    {
        if (queue.Count == 0)
            return;

        // Remove the current highest Momentum StatSheet
        StatSheet currentSheet = queue[0];
        queue.RemoveAt(0);

        // Perform action for currentSheet here (e.g., reduce its Momentum, add delay, etc.)
        // Assume we have a method `PerformAction` which updates the Momentum:
        currentSheet.PerformAction(); // Modify Momentum as part of taking a turn

        // Re-insert currentSheet at its new position in the queue.
        InsertSheet(currentSheet);
    }

    // Insert a StatSheet back into the queue based on updated Momentum.
    private void InsertSheet(StatSheet sheet)
    {
        // Use binary search to find the correct position efficiently
        int index = queue.BinarySearch(sheet, new MomentumComparer());

        // BinarySearch returns a negative index if no match is found; adjust for insertion
        if (index < 0)
            index = ~index;

        queue.Insert(index, sheet); // Insert at the correct position based on Momentum.
    }

    // Custom comparer for BinarySearch to sort StatSheets by Momentum in descending order.
    private class MomentumComparer : IComparer<StatSheet>
    {
        public int Compare(StatSheet x, StatSheet y)
        {
            return y.getMomentumCurrent().CompareTo(x.getMomentumCurrent()); // Descending
        }
    }
}

*/
