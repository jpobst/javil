package com.example;

class GenericNestedType<A, B>
{
    class Nested<C> extends GenericNestedType<A, C> 
    {
        public void doThing (A value, B value2, C value3) { } 
    }

    public void doThing2 (java.util.Map<? extends A, ? extends B> map) { }

    public void doThing3 (A key, java.util.function.Function<? super A, ? extends B> mappingFunction) { }

    public void doThing4 (Class<?>[] classes) { }
}
