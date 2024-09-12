using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataManager : SingletonBehaviour<UserDataManager>
{
   //����� ���� ������ ���� ����
   public bool ExistSavedData {  get; private set; }
    //��� ���� ������ �ν��Ͻ��� �����ϴ� �����̳�
    public List<IUserData> UserDataList { get; private set; } = new List<IUserData>();

    protected override void Init()
    {
        base.Init();

        //��� ���� �����͸� UserDataList�� �߰�
        UserDataList.Add(new UserSettingsData());
        UserDataList.Add(new UserGoodsData());
    }

    public void SetDefaultUserData()
    {
        for(int i = 0; i < UserDataList.Count; i++)
        {
            UserDataList[i].SetDefaultData();
        }
    }

    public void LoadUserData()
    {
        ExistSavedData = PlayerPrefs.GetInt("ExistsSavedData") == 1 ? true : false;

        if(ExistSavedData)
        {
            for(int i = 0; i  < UserDataList.Count; i++)
            {
                UserDataList[i].LoadData();
            }
        }

    }

    public void SaveUserData()
    {
        bool hasSaveError = false;

        for(int i = 0; i < UserDataList.Count; i++)
        {
            bool isSavedSuccess =  UserDataList[i].SaveData();
            if(!isSavedSuccess)
            {
                hasSaveError = true;
            }
        }

        if(!hasSaveError)
        {
            ExistSavedData = true;
            PlayerPrefs.SetInt("ExistsSavedData", 1);
            PlayerPrefs.Save();
        }



    }




}
