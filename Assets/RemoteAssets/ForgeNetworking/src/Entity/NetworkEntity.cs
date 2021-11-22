using Forge.Networking.Players;
using Forge.Networking.Unity.Messages;
using Forge.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Forge.Networking.Unity
{
	public class NetworkEntity : MonoBehaviour, IUnityEntity
	{		
		[SerializeField] private string _sceneIdentifier = null;
		[SerializeField] private int _prefabId = -1;
		[SerializeField, HideInInspector] private int _sceneIndex = 0;
		public int Id { get; set; }
		public IPlayerSignature OwnerId { get; set; }
		public int PrefabId { get { return _prefabId; } set { } }
		public GameObject OwnerGameObject => gameObject;
		public int SceneIndex => _sceneIndex;
		public string SceneIdentifier
		{
			get => _sceneIdentifier;
			set { _sceneIdentifier = value; }
		}

		public IEngineFacade engine { get; set; }

		private void Awake()
		{
			_sceneIndex = SceneManager.GetActiveScene().buildIndex;
		}

		private void OnDestroy()
		{
			if (engine.IsServer)
            {
				var despawnMessage = new DespawnEntityMessage();
				despawnMessage.EntityId = Id;				

				engine.NetworkMediator.SendReliableMessage(despawnMessage);

				if (engine.EntityRepository.Exists(this.Id))
					engine.EntityRepository.Remove(this);
			}
		}

		private void OnValidate()
		{
			// OnValidate is only called in the editor
			if (Application.isPlaying) return;

			var entities = GameObject.FindObjectsOfType<NetworkEntity>();
			List<string> _currentIds = entities.Where(e => e != this).Select(e => e._sceneIdentifier).ToList();
			foreach (var e in entities)
			{
				if (e == this)
				{
					if (string.IsNullOrEmpty(_sceneIdentifier))
						_sceneIdentifier = Guid.NewGuid().ToString();
					else if (_currentIds.Contains(e._sceneIdentifier))
						e._sceneIdentifier = Guid.NewGuid().ToString();
					break;
				}
			}
		}
	}
}
