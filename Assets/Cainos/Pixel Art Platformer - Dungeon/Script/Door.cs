using UnityEngine;
using Cainos.LucidEditor;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


namespace Cainos.PixelArtPlatformer_Dungeon
{
    public class Door : MonoBehaviour
    {
        [FoldoutGroup("Reference")] public SpriteRenderer spriteRenderer;
        [FoldoutGroup("Reference")] public Sprite spriteOpened;
        [FoldoutGroup("Reference")] public Sprite spriteClosed;
        [FoldoutGroup("Reference")] public BoxCollider2D frameCollider; // Référence au BoxCollider2D du GameObject Frame

        [FoldoutGroup("Audio")] public AudioClip openSound; // Son d'ouverture
        [FoldoutGroup("Audio")] public AudioClip closeSound; // Son de fermeture

        private AudioSource audioSource; // Référence à l'AudioSource
        private Animator animator;
        private bool isOpened = false; // État actuel de la porte

        private Animator Animator
        {
            get
            {
                if (animator == null) animator = GetComponent<Animator>();
                return animator;
            }
        }

        void Start()
        {
            animator = GetComponent<Animator>(); // Récupérer l'Animator attaché à la porte
            audioSource = GetComponent<AudioSource>(); // Récupérer l'AudioSource attaché à la porte
            Animator.Play(isOpened ? "Opened" : "Closed");
            IsOpened = isOpened;

            if (frameCollider == null)
            {
                // Récupérer automatiquement le BoxCollider2D du GameObject enfant "Frame" s'il n'est pas assigné
                Transform frameTransform = transform.Find("Frame");
                if (frameTransform != null)
                {
                    frameCollider = frameTransform.GetComponent<BoxCollider2D>();
                }
            }
        }

        [FoldoutGroup("Runtime"), ShowInInspector]
        public bool IsOpened
        {
            get { return isOpened; }
            set
            {
                isOpened = value;

                #if UNITY_EDITOR
                if (Application.isPlaying == false)
                {
                    EditorUtility.SetDirty(this);
                    EditorSceneManager.MarkSceneDirty(gameObject.scene);
                }
                #endif

                if (Application.isPlaying)
                {
                    Animator.SetBool("IsOpened", isOpened);
                    if (frameCollider != null) frameCollider.enabled = !isOpened; // Activer/désactiver le BoxCollider2D du cadre

                    // Jouer le son approprié
                    if (audioSource != null)
                    {
                        AudioClip soundToPlay = isOpened ? openSound : closeSound;
                        if (soundToPlay != null)
                        {
                            audioSource.PlayOneShot(soundToPlay);
                        }
                    }
                }
                else
                {
                    if(spriteRenderer) spriteRenderer.sprite = isOpened ? spriteOpened : spriteClosed;
                }
            }
        }

        [FoldoutGroup("Runtime"), HorizontalGroup("Runtime/Button"), Button("Open")]
        public void Open()
        {
            if (!isOpened) // Si la porte n'est pas déjà ouverte
            {
                IsOpened = true;
                Debug.Log("Porte ouverte."); // Débogage
            }
        }

        [FoldoutGroup("Runtime"), HorizontalGroup("Runtime/Button"), Button("Close")]
        public void Close()
        {
            if (isOpened) // Si la porte est ouverte
            {
                IsOpened = false;
                Debug.Log("Porte fermée."); // Débogage
            }
        }

        [FoldoutGroup("Runtime"), HorizontalGroup("Runtime/Button"), Button("Toggle")]
        public void Toggle()
        {
            if (isOpened)
            {
                Close(); // Fermer la porte si elle est ouverte
            }
            else
            {
                Open(); // Ouvrir la porte si elle est fermée
            }
        }
    }
}
