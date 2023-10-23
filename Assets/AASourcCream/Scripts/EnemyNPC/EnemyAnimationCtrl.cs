using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;



public class EnemyAnimationCtrl : SerializedMonoBehaviour
{

    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
    public Dictionary<string, string> attacksRegular;
    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
    public Dictionary<string, string> attacksSpecial;
    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
    public Dictionary<string, string> extra;

    [SerializeField]
    public string RunAnimation;
    [SerializeField]
    public string walkAnimation;

    private Animator animator;

    [SerializeField]
    private RuntimeAnimatorController animatorController;

    public string _currentState;

    private string currentAnimationName;

    bool hasAttacked = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        animator = this.GetComponent<Animator>();
        //animator.applyRootMotion = true;
    }

    // Update is called once per frame
    void Update()
    {
        // if (!once) {
        //     _animator.Play("Goblin_slash", -1);
        //     //_animator.Play("death", -1);
        //     Debug.Log(LayerMask.NameToLayer("Base Layer"));
        //     once = true;
        // }
    }

    //TODO use this https://docs.unity3d.com/ScriptReference/Animator.CrossFade.html crossFade seems to be smooth transition playing
    public void PlayAnimation(string name)
    {
        if (currentAnimationName != name) {
            Debug.Log(name);
            currentAnimationName = name;
            animator.CrossFade(name, 0.2f);
        }
    }

    public void SetAnimatorVar(string varName, bool val)
    {
        Debug.Log(val);
        Debug.Log(varName);
        animator.SetBool(varName, val);
    }
}