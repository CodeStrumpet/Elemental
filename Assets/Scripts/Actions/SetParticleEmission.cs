using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Particles")]
	[Tooltip("Set particle emission size")]
	public class SetParticleEmission : FsmStateAction
	{
	    [RequiredField]
		[CheckForComponent(typeof(ParticleEmitter))]
		public FsmOwnerDefault gameObject;

	    public FsmInt minEmission;
	    public FsmInt maxEmission;

	    public override void Reset()
	    {
		minEmission = 5;
		maxEmission = 5;
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

		emitter.minEmission = minEmission.Value;
		emitter.maxEmission = maxEmission.Value;
	    }
	}
}