package com.example;

class BaseMethodBaseClass
{
    public Object doThing () { return null; }
    public Object doThing (Object obj) { return null; }
    public Object doThing (int value) { return null; }

    public int doThing2 () { return 0; }

    public static Object doThing3 () { return null; }

    public <A> A doThing4 (A value, java.util.List<A> list) { return null; }

    public <A> A doThing5 (A value, java.util.List<? super A> list) { return null; }
}
