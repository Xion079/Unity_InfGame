using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    //로고
    public Animation LogoAnim;
    public TextMeshProUGUI LogoText;

    //타이틀
    public GameObject Title;
    public Slider LoadingSlider;
    public TextMeshProUGUI LoadingProgressText;

    private AsyncOperation m_AsyncOperation;


    private void Awake()
    {
        LogoAnim.gameObject.SetActive(true);
        Title.SetActive(false);
    }

    private void Start()
    {
        //유저 데이터 로드
        UserDataManager.Instance.LoadUserData();

        //저장된 유저 데이터가 없으면 기본값으로 세팅 후 저장
        if(!UserDataManager.Instance.ExistSavedData)
        {
            UserDataManager.Instance.SetDefaultUserData();
            UserDataManager.Instance.SaveUserData();
        }

        var confirmUIData = new ConfirmUIData();
        confirmUIData.ConfirmType = ConfirmType.OK;
        confirmUIData.TitleTxt = "UI Test";
        confirmUIData.DescTxt = "This is UI Test. ";
        confirmUIData.OKBtnTxt = "OK";
        UIManager.Instance.OpenUI<ConfirmUI>(confirmUIData);

        //ChapterData chapterData1 = DataTableManager.Instance.GetChapterData(10);
        //ChapterData chapterData2 = DataTableManager.Instance.GetChapterData(50);

        //return;

        StartCoroutine(LoadGameCo());
    }

    private IEnumerator LoadGameCo()
    {
        Logger.Log($"{GetType()}::LoadGameCo");

        AudioManager.Instance.PlayBGM(BGM.lobby);
        yield return new WaitForSeconds(5f);
        AudioManager.Instance.PauseBGM();
        yield return new WaitForSeconds(5f);
        AudioManager.Instance.ResumeBGM();
        yield return new WaitForSeconds(5f);
        AudioManager.Instance.StopBGM();

        LogoAnim.Play();
        yield return new WaitForSeconds(LogoAnim.clip.length);

        LogoAnim.gameObject.SetActive(false);
        Title.SetActive(true);

        m_AsyncOperation = SceneLoader.Instance.LoadSceneAsync(SceneType.Lobby);
        if(m_AsyncOperation == null)
        {
            Logger.Log("Lobby async loading error.");
            yield break;
        }

        m_AsyncOperation.allowSceneActivation = false;

        LoadingSlider.value = 0.5f; //0이 아닌이유가 로딩이 빠른 게임이면 0에서 시작하면 너무 빨라서 0.5로 한거임
        LoadingProgressText.text = $"{(int)(LoadingSlider.value * 100)}%";
        yield return new WaitForSeconds(0.5f);

        while(!m_AsyncOperation.isDone) //로딩이 진행 중일 때
        {
            LoadingSlider.value = m_AsyncOperation.progress < 0.5f ? 0.5f : m_AsyncOperation.progress;
            LoadingProgressText.text = $"{(int)(LoadingSlider.value * 100)}%";

            //씬 로딩이 완료되었다면 로비로 전환하고 코루틴 종료
            if (m_AsyncOperation.progress >= 0.9f)
            {
                m_AsyncOperation.allowSceneActivation = true;
                yield break;
            }

            yield return null;
        }

    }




}
