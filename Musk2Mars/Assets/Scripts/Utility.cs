using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Utility {

    /**
    * Retrun a delegate with return type 'object'
    * If want to use a method which returns void, make that method return a null object instead
    */
    public static Func<object> BuildMethod<T> (Func<T> f) {
        return () => f();
    }
}