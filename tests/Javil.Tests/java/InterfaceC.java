package com.example;

interface InterfaceC extends InterfaceA {
	default int add (int param1, int param2) { return param1 + param2; }
}