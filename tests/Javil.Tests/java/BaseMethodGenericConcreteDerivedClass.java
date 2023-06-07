package com.example;

class BaseMethodGenericConcreteDerivedClass extends BaseMethodGenericBaseClass<Object>
{
    @Override
    public Object doThing () { return null; }

    @Override
    public BaseMethodGenericConcreteDerivedClass doThing (Object obj) { return null; }

    @Override
    public void doThing4 (Object value) { }

    @Override
    public Object doThing5 (Object value) { return null; }

    @Override
    public BaseMethodGenericConcreteDerivedClass doThing6 (Object value) { return null; }

    @Override
    public void doThing7 (java.util.List<Object> value) { }

    @Override
    public void doThing8 (java.util.List<Object> value) { }
}
