using UnityEngine;
using System.Collections;

public class AnimationController2D : MonoBehaviour {


	private string _currentAnimation;
	private Animator _animator;

	void Awake () {
		_animator = this.GetComponent<Animator>();

	}

	public void setAnimation (string animationName){

		int hash = Animator.StringToHash(animationName);

		if (_animator != null && _animator.HasState(0,hash)){
			//Check that we're not already playing the animation 
			if ( animationName != _currentAnimation ) {
				
				//Set the animation to play in the animator
				_animator.Play(hash);
				
				//Update the member variable that tracks the character action state
				_currentAnimation = animationName;
			}
		}
	}
}
