using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("SFX Settings")]
    public AudioSource sfxSource;  // AudioSource koji će se koristiti za SFX
    public AudioClip[] sfxClips;   // Lista svih SFX klipova (ovo ćeš postaviti u Unity editoru)
    [Range(0f, 1f)]
    public float[] sfxVolumes;     // Niz glasnoće za svaki SFX (postavi vrednosti između 0 i 1)

    [Header("Global Volume Control")]
    [Range(0f, 1f)]
    public float globalVolume = 1f;  // Globalna glasnoća (podešava sve SFX-ove)

    [Header("Music Settings")]
    public AudioSource musicSource;  // AudioSource koji će se koristiti za muziku
    public AudioClip musicClip;      // Pesma koja će biti puštana u petlji
    [Range(0f, 1f)]
    public float musicVolume = 1f;   // Glasnoća muzike

    private void Awake()
    {
        // Singleton pattern, osiguraj da postoji samo jedna instanca AudioManagera
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Ovaj objekat neće biti uništen pri promeni scene
        }

        // Osiguraj da je glasnoća postavljena na globalnu vrednost za SFX
        if (sfxVolumes.Length != sfxClips.Length)
        {
            sfxVolumes = new float[sfxClips.Length]; // Podesi glasnoću svakog efekta na početnu vrednost
            for (int i = 0; i < sfxVolumes.Length; i++)
            {
                sfxVolumes[i] = 1f;  // Podrazumevana glasnoća svakog SFX-a je maksimalna
            }
        }

        // Postavljanje muzike u petlju
        if (musicSource != null && musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.loop = true;  // Postavljanje muzike da se ponavlja
            musicSource.volume = musicVolume; // Podesi glasnoću muzike
            musicSource.Play();  // Pusti muziku
        }
    }

    // Funkcija za puštanje SFX-a na osnovu indeksa u nizu
    public void PlaySFX(int clipIndex)
    {
        if (clipIndex >= 0 && clipIndex < sfxClips.Length)
        {
            // Podesi glasnoću za trenutni SFX uzimajući u obzir globalnu glasnoću
            float finalVolume = sfxVolumes[clipIndex] * globalVolume;
            sfxSource.PlayOneShot(sfxClips[clipIndex], finalVolume);
        }
        else
        {
            Debug.LogWarning("SFX clip index out of range.");
        }
    }

    // Funkcija za podešavanje globalne glasnoće svih SFX-a
    public void SetGlobalVolume(float volume)
    {
        globalVolume = Mathf.Clamp(volume, 0f, 1f);  // Ograniči vrednost između 0 i 1
    }

    // Funkcija za podešavanje glasnoće specifičnog SFX-a
    public void SetSFXVolume(int clipIndex, float volume)
    {
        if (clipIndex >= 0 && clipIndex < sfxVolumes.Length)
        {
            sfxVolumes[clipIndex] = Mathf.Clamp(volume, 0f, 1f);  // Ograniči glasnoću između 0 i 1
        }
        else
        {
            Debug.LogWarning("SFX clip index out of range.");
        }
    }

    // Funkcija za podešavanje glasnoće muzike
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp(volume, 0f, 1f);  // Ograniči glasnoću između 0 i 1
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }
}
