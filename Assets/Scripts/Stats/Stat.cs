using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;


public class Stat
{
    public string name;
    public int max;
    public bool isStaticValue = false;
    public int current;
    public int buff;
    public int debuff;

    public Stat(int initial)
    {
        max = initial;
        current = initial;
        isStaticValue = false;
    }

    public string getName() {

        return name;
    }

    public void setName(string name)
    {
        this.name = name;
    }

    public int getMax()
    {
        return max;
    }

    public int getCurrent()
    {
        int current = this.current + buff - debuff;
        //Hokay... so... if it's a static value, the minimum is 1 and it has no maximum
        //If it's not a static value, then it need a maximum and its minimum is 0
        int min = isStaticValue ? 1 : 0;

        if (current < min)
            return min;

        if (!isStaticValue && current > max)
            return max;

        return current;

    }

    public void setMax(int max)
    {
        this.max = max;
    }


    public void setCurrent(int current)
    {
        this.current = current;
    }


    public int getMin()
    {
        if (isStaticValue == true)
            return 1;
        else
            return 0;
    }

    public void setStaticValue(bool value)
    {
        isStaticValue = value;
    }
}
