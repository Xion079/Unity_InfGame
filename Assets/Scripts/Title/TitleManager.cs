using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    //�ΰ�
    public Animation LogoAnim;
    public TextMeshProUGUI LogoText;

    //Ÿ��Ʋ
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
        //���� ������ �ε�
        UserDataManager.Instance.LoadUserData();

        //����� ���� �����Ͱ� ������ �⺻������ ���� �� ����
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

        LoadingSlider.value = 0.5f; //0�� �ƴ������� �ε��� ���� �����̸� 0���� �����ϸ� �ʹ� ���� 0.5�� �Ѱ���
        LoadingProgressText.text = $"{(int)(LoadingSlider.value * 100)}%";
        yield return new WaitForSeconds(0.5f);

        while(!m_AsyncOperation.isDone) //�ε��� ���� ���� ��
        {
            LoadingSlider.value = m_AsyncOperation.progress < 0.5f ? 0.5f : m_AsyncOperation.progress;
            LoadingProgressText.text = $"{(int)(LoadingSlider.value * 100)}%";

            //�� �ε��� �Ϸ�Ǿ��ٸ� �κ�� ��ȯ�ϰ� �ڷ�ƾ ����
            if (m_AsyncOperation.progress >= 0.9f)
            {
                m_AsyncOperation.allowSceneActivation = true;
                yield break;
            }

            yield return null;
        }

    }




}
