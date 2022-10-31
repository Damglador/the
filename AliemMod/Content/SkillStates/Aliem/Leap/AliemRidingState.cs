﻿namespace ModdedEntityStates.Aliem {

    public class AliemRidingState : BaseRidingState {

        public override void OnEnter() {
            base.OnEnter();

			PlayAnimation("Fullbody, Override", "Riding");
        }

        public override void FixedUpdate() {
			base.FixedUpdate();

			if (inputBank.jump.justPressed) {
				//todo jump off
				riddenBody.RemoveBuff(Modules.Buffs.riddenBuff);
				base.outer.SetState(new AliemCharacterMain { wasRiding = true });

				PlayAnimation("FullBody, Override", "BufferEmpty");
				return;
			}

			if (inputBank.skill1.justPressed) {
				//todo chomp
				riddenBody.RemoveBuff(Modules.Buffs.riddenBuff);

				AliemRidingChomp chompState = new AliemRidingChomp();
				chompState.riddenBody = this.riddenBody;

				base.outer.SetState(chompState);
				return;
			}
		}
    }
}
