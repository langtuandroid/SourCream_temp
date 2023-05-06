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

    private Animator _animator;

    [SerializeField]
    private RuntimeAnimatorController animatorController;

    public string _currentState;

    bool hasAttacked = false;
    // Start is called before the first frame update
    void Start()
    {
        _animator.Play("idleDaggers");
    }

    private void Awake()
    {
        _animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void changeAnimationState(string newState)
    {
        if (newState == _currentState) {
            return;
        }
        _animator.Play(newState);
        _currentState = newState;
    }

    // //0 is animation layer
    // public bool isAnimationPlaying(string name)
    // {
    //     if (_animator.GetCurrentAnimatorStateInfo(0).IsName(name) && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
    //     {
    //         return true;
    //     }
    //     else
    //     {
    //         return false;
    //     }
    // }

    //TODO use this https://docs.unity3d.com/ScriptReference/Animator.CrossFade.html crossFade seems to be smooth transition playing
    public float PlayAnimation(string name)
    {
        var currentClip = _animator.GetCurrentAnimatorClipInfo(0);

        _animator.CrossFade("Base Layer." + name, 0.2f);
        return _animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
    }
}
