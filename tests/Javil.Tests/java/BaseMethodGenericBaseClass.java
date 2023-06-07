package com.example;

class BaseMethodGenericBaseClass<T>
{
    public T doThing () { return null; }
    public T doThing (Object obj) { return null; }
    public T doThing (int value) { return null; }

    public int doThing2 () { return 0; }

    public static Object doThing3 () { return null; }

    public void doThing4 (T value) { }

    public T doThing5 (T value) { return null; }

    public T doThing6 (T value) { return null; }

    public void doThing7 (java.util.List<T> list) { }

    public void doThing8 (java.util.List<? super T> list) { }
}
