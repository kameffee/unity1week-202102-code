using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domain;
using Master;
using UnityEngine;
using Utility;
using VContainer;

namespace UseCase
{
    /// <summary>
    /// メッセージイベントの操作
    /// </summary>
    public class MessageEventUseCase
    {
        private readonly IMessageDatabase messageDatabase;
        private readonly MessageModel messageModel;

        public bool IsPlaying { get; private set; }

        [Inject]
        public MessageEventUseCase(IMessageDatabase messageDatabase, MessageModel messageModel)
        {
            this.messageDatabase = messageDatabase;
            this.messageModel = messageModel;
        }

        public async UniTask StartMessage(int id, bool autoClose = true, CancellationToken cancellationToken = default)
        {
            IMessageData data = messageDatabase.FindById(id);
            if (data == null)
            {
                Debug.LogError($"Message Not found. id:{id}");
                return;
            }

            await StartMessage(data, autoClose, cancellationToken);
        }

        public async UniTask StartMessage(IMessageData data, bool autoClose = true,
            CancellationToken cancellationToken = default)
        {
            IsPlaying = true;

            foreach (var messageEntity in data.MessageList)
            {
                await messageModel.StartMessage(messageEntity.message, cancellationToken);
                await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: cancellationToken);
            }

            if (autoClose)
            {
                // 閉じる
                messageModel.SetVisible(false);
            }

            IsPlaying = false;
        }

        public async UniTask RandomMessage()
        {
            var data = messageDatabase.All().Random();
            await StartMessage(data);
        }
    }
}