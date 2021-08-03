# `Alchera Face Hand Detection Unity`

## What does this project do?

We are distributing the Alchera Unity Plugin for the purpose of seeing how our Plugin meets the market's demand. Alchera Unity Plugin is a Unity3D Project that detects faces and hands in 2D/3D. <br>
You can test this project on Android and iOS, or, just download Alchera app <a href="https://play.google.com/store/apps/details?id=com.alcherainc.alunityplugin" target="_blank"><img width="25" src="https://i.imgur.com/e8wjusG.png"/></a>  from google playstore for testing. 

First of all, thank you for your interest in Alchera Unity Plugin. <br>
Alchera Unity Plugin was supported until 2021.06.30 and was successfully distributed via Google form. Further distribution is being prepared and we will get back to you at our earliest. <br>
<br>
Those applicants who applied by the end of June will receive a separate email regarding an additional use of license with a request for a survey completion. For new applicants, additional license usage permission can be applied via the README link that will be updated by August. <br>
Below is the brief schedule of our Alchera Unity Plugin distribution. 
- License will be distributed on mid of August 2021 to applicants. 
- License will be redistributed after January 2022 with an additional survey. The SDK model license will extend until August 2022. 


We would (sincerely) ask for your interest in our upcoming SDK distribution. <br>
Thank you <br>

---
### What's in it?
|                         Face2DFacemark                          |                          Face3DSticker                          |                          Face3DAnimoji                          |
| :-------------------------------------------------------------: | :-------------------------------------------------------------: | :-------------------------------------------------------------: |
| ![](https://media.giphy.com/media/Uov097lvdnXrq7cS5L/giphy.gif) | ![](https://media.giphy.com/media/S6Z48wBxLRajZTzrfQ/giphy.gif) | ![](https://media.giphy.com/media/eIaOn6eLvzBBIT4Uzf/giphy.gif) |

|                           Face3DMask                            |                         Hand2DSkeleton                          |                       Complex(face+hand)                        |
| :-------------------------------------------------------------: | :-------------------------------------------------------------: | :-------------------------------------------------------------: |
| ![](https://media.giphy.com/media/WOHY3NvxNv6MFxAjxx/giphy.gif) | ![](https://media.giphy.com/media/dsdRPOxwqMDDqX24sU/giphy.gif) | ![](https://media.giphy.com/media/QBwRPVOlP2EIUsBSIR/giphy.gif) |

---
<details>
  <summary>Open<h3>What is the scope of your application?<h3></summary>
  
> Device Orientation
> - [x] Portrait
> - [ ] Portrait Upside Down
> - [x] Landscape Right
> - [x] Landscape Left

> Camera
> - [x] Front Camera
> - [x] Rear Camera

> Platform
> - [x] Androidㅤ`ARMv7`, `ARM64`
> - [x] iOSㅤㅤㅤ`arm64`, `armv7`, `armv7s`

> Max detection count
> - [x] Unlimited. `The more, the slower.`
</details>


---
<details>
  <summary>Open<h3>How do I run the test?<h3></summary>
    
    Click image below to see Youtube tutorial.
[![Video Label](https://i.imgur.com/9dLzsm3.png)](https://www.youtube.com/watch?v=tSU9wG1huhU)<br>

> 1. **Clone** or **Download** this repository.

> 2. open it with **Unity3D**<br>
> we've tested with Unity3D version **`2019.2.13f`** `Universal RP`.<br>
> `Universal RP` is optional, but without setup, graphics can be broken as follows:<br>
> <img width="600" src="https://i.imgur.com/vNeZtmm.png"/><br>
> If you don't mind, it will work in the 2018 as well.
 
> 3. Add model files you received in Resources folder<br>
> Android<br>
> <img width="500" src="https://user-images.githubusercontent.com/49930641/108683708-51bd2980-7535-11eb-99ae-6442341cff9a.PNG"/><br>
> iOS<br>
> <img width="500" src="https://user-images.githubusercontent.com/49930641/108683163-9399a000-7534-11eb-84de-c8177404ae87.PNG"/><br>


> 4. If you want to play with **`iOS`**. unzip [opencv2.framework](https://github.com/AlcheraInc/Alchera-Face-Hand-Detection-Unity/releases/download/opencv2.frameworks/opencv2.framework.zip) to Assets/Alchera/Plugins/iOS<br>
> (It is too large for github push)<br>
> <img width="500" src="https://i.imgur.com/OLnMasu.png"/><br>
> Make sure the platform is checked with iOS.<br>

> 5. Set Unity3D settings.<br>
> `Window - Package Manager`<br>
> <img width="500" src="https://i.imgur.com/HbmbiEA.png"/><br>
> We use Universal RP 6.9.2<br><br>
> `File - Build Settings`<br>
> <img width="500" src="https://i.imgur.com/5VGvf8E.png"/><br>
> Place the `Splash`scene first and the `DemoUI`scene second.<br><br>
> `Edit - Project Settings - Graphics`<br>
> <img width="500" src="https://i.imgur.com/qalWxXS.png"/><br>
> Set `Scriptable Render PipelineAsset` to **`LightweightAsset`**. for `Universal RP`<br><br>
> `Edit - Project Settings - Player - Other Settings - iOS`<br>
> <img width="500" src="https://i.imgur.com/K83k8Sz.png"/><br>
> `Edit - Project Settings - Player - Other Settings - Android`<br>
> <img width="500" src="https://i.imgur.com/62GRibJ.png"/><br>

> 6. Build And Run.<br>
> build with `iOS` or `Android`. And see the **`magic :)`**
</details>
