package com.example;

class GenericDerivedNestedType<A, B> extends GenericNestedType<Object, Object>
{
    @Override
    public void doThing2 (java.util.Map<?, ?> map) { }

    @Override
    public void doThing3 (Object key, java.util.function.Function<? super Object, ?> mappingFunction) { }

    @Override
    public void doThing4 (Class[] classes) { }
}
