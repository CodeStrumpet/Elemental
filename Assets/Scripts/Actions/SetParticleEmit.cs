using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Particles")]
	[Tooltip("Activates or deactivates particle emit")]
	public class SetParticleEmit : FsmStateAction
	{
	    [RequiredField]
		[CheckForComponent(typeof(ParticleEmitter))]
		public FsmOwnerDefault gameObject;

	    [RequiredField]
		public FsmBool emit;

	    public override void Reset()
	    {
		emit = false;
	    }

	    public override void OnEnter()
	    {
		Finish();

		var go = Fsm.GetOwnerDefaultTarget(gameObject);
		if (go == null) {
		    return;
		}

		var emitter = go.GetComponent(typeof(ParticleEmitter)) as ParticleEmitter;
		if (emitter == null) {
		    LogWarning("Missing particle emitter: " + go.name);
		    return;
		}

		emitter.emit = emit.Value;
	    }
	}
}