package com.example;

class BaseMethodDerivedClass extends BaseMethodBaseClass
{
    @Override
    public Object doThing () { return null; }

    @Override
    public BaseMethodDerivedClass doThing (Object obj) { return null; }

    public static Object doThing3 () { return null; }

    @Override
    public <A> A doThing4 (A value, java.util.List<A> list) { return null; }

    @Override
    public <A> A doThing5 (A value, java.util.List<? super A> list) { return null; }

    //@Override
    //public void doThing6 (java.util.Map<Object, Object> map) { }

}
