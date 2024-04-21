using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum UserType
{
    UNDEFINED,
    PLAYER,
    HOST,
    CHICKEN
}
[System.Serializable]
public class User
{
    public string userName;

    public string password;

    public UserType playerType;

}

[CreateAssetMenu(fileName = "ProfileHolder", menuName = "ScriptableObjects/Network/Profile", order = 1)]
public class ProfileHolder : ScriptableObject
{
    [SerializeField]
    public List<User> users;


}
