# KGI API Wrapper

這個project提供一組wrapper給KGI的電子下單API wrapper，請自行上凱基證券申請API帳號以及權限。主要是以.NET core為主開發核心以Deploy上linux server為主要目的。

## 安裝

- 使用 `KGIECTrade.exe --create-config` 產生`config.yaml`
- 修改裡面的ID跟Password

## 計畫功能

### Local端提供的功能

- 提供策略下單功能，實作IStrategyTask即可策略下單
	- 不過很遺憾，IStrategyTask還是得用C#自己實作 XD
- Daily Report
- 提供手動下單功能
- 提供監視功能
- 提供email警訊發報的功能
- Logger

### Web端提供的

- Wrap sessions
- 

