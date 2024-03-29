﻿using System;
using System.Collections.Generic;
using System.Text;



/*
 * Extends on the Java solutions provided in week 3 since it's a fascinating way to write code. 
 * 
 * We'll leave it here
 * 
 */

/**
 * A State contains information regarding how the "world" looks at a given point. <T> is generic, allowing States to contain any class
 * 
 * @author Steven Morris
 * @version 21/03/2016
 */

namespace robot_nagivation
{
    public abstract class StateData
    {
        public abstract int GetCost();
    }

    public class State<T>
    {
        private T _data;
        private State<T> _parent;
        private string _message;
        private int _cost;


        public State(State<T> parent, String message, T data)
        {
            _parent = parent;
            _message = message;
            _data = data;
        }

        public State(State<T> parent, String message, T data, int cost):
            this(parent, message, data)
        {
            _cost = cost;
        }


        public State<T> Parent { get => _parent; }
        public string Message { get => _message; }
        public int Cost { get => _cost; }
    }

    public abstract class Scenario<T>
    {
        public abstract List<T> DeterminePossibleMoves(State<T> state);

        public abstract bool isSolved(T data);

        public void LogSolution(State<T> state)
        {
            Stack<String> messageStack = new Stack<string>();
            while (state != null)
            {
                messageStack.Push(state.Message);
                state = state.Parent;
            }

            while(messageStack.Count > 0)
            {
                Console.WriteLine(messageStack.Pop());
            }
        }
    }
}
