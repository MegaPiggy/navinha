﻿using UnityEngine;

namespace Navinha
{
    class NaveFlightConsole : MonoBehaviour
    {
        public OWRigidbody naveBody;

        private InteractVolume interactVolume;

        private PlayerAttachPoint attachPoint;

        //private OWRigidbody attachedBody;

        private AudioSource playerAudioSource;

        private ScreenPrompt promptDeAtivar;

        private ScreenPrompt promptDeReduzirPotencia;

        private ScreenPrompt promptDeAumentarPotencia;

        public ScreenPrompt valorDaPotencia;
        public NaveThrusterController naveThrusterController;


        private void Awake()
        {
            enabled = false;
            #if ALPHA_VERSION
            promptDeAtivar = new ScreenPrompt(XboxButton.LeftStick, "Ligar", 1);
            promptDeReduzirPotencia = new ScreenPrompt(XboxButton.LeftTrigger, "Diminuir Potencia", 1);
            promptDeAumentarPotencia = new ScreenPrompt(XboxButton.RightTrigger, "Aumentar Potencia", 1);
            valorDaPotencia = new ScreenPrompt("Potencia 0%", 1);
            #elif CURRENT_VERSION
            promptDeAtivar = new ScreenPrompt(InputLibrary.thrustZ, "Ligar", 1);
            promptDeReduzirPotencia = new ScreenPrompt(InputLibrary.thrustDown, "Diminuir Potencia", 1);
            promptDeAumentarPotencia = new ScreenPrompt(InputLibrary.thrustUp, "Aumentar Potencia", 1);
            valorDaPotencia = new ScreenPrompt("Potencia 0%", 1);
            #endif

            attachPoint = this.GetRequiredComponent<PlayerAttachPoint>();
            interactVolume = this.GetRequiredComponent<InteractVolume>();
            playerAudioSource = gameObject.GetTaggedComponent<AudioSource>("Player");

            interactVolume.OnPressInteract += OnPressInteract;
        }

        private void OnDestroy()
        {
            interactVolume.OnPressInteract -= OnPressInteract;
        }

        private void OnPressInteract()
        {
            if (!enabled)
            {
                attachPoint.AttachPlayer();
                enabled = true;
                GlobalMessenger<OWRigidbody>.FireEvent("EnterNaveFlightConsole", naveBody);
                #if ALPHA_VERSION
                Locator.GetPromptManager().AddScreenPrompt(promptDeAtivar, PromptPosition.left, true);
                Locator.GetPromptManager().AddScreenPrompt(promptDeReduzirPotencia, PromptPosition.left, true);
                Locator.GetPromptManager().AddScreenPrompt(promptDeAumentarPotencia, PromptPosition.left, true);
                Locator.GetPromptManager().AddScreenPrompt(valorDaPotencia, PromptPosition.bottom, true);
                #elif CURRENT_VERSION
                Locator.GetPromptManager().AddScreenPrompt(promptDeAtivar, PromptPosition.LowerLeft, true);
                Locator.GetPromptManager().AddScreenPrompt(promptDeReduzirPotencia, PromptPosition.LowerLeft, true);
                Locator.GetPromptManager().AddScreenPrompt(promptDeAumentarPotencia, PromptPosition.LowerLeft, true);
                Locator.GetPromptManager().AddScreenPrompt(valorDaPotencia, PromptPosition.BottomCenter, true);
                #endif
            }
        }

        private void Update()
        {
            if (OWInput.GetButtonUp(InterfaceInput.cancel))
            {
                attachPoint.DetachPlayer();
                interactVolume.ResetInteraction();
                enabled = false;
                GlobalMessenger.FireEvent("ExitNaveFlightConsole");
                Locator.GetPromptManager().RemoveScreenPrompt(promptDeAtivar);
                Locator.GetPromptManager().RemoveScreenPrompt(promptDeReduzirPotencia);
                Locator.GetPromptManager().RemoveScreenPrompt(promptDeAumentarPotencia);
                Locator.GetPromptManager().RemoveScreenPrompt(valorDaPotencia);
            }
            if (naveThrusterController != null)
                valorDaPotencia.SetText("Potencia: " + naveThrusterController.Potencia / 10 + '%');
        }
    }
}
