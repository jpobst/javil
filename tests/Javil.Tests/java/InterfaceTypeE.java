package com.example;

 class InterfaceTypeE implements InterfaceA, InterfaceD, MyComparable<InterfaceTypeE> {
    // This class does not have to implement InterfaceA.add because InterfaceD implements InterfaceC provides a default implementation of it

    public int compareTo (InterfaceTypeE p0) { return 0; }
}