using System;
using System.IO;

namespace CnDCopy.Kernel
{
    public class Copier
    {
        public static ReplaceMode DefaultReplaceMode { get; set; }
        public ILocationFactory LocationFactory { get; set; }
        public Func<ReplaceMode> UserAskHandler { private get; set; } 

        public bool Copy(ILocation source, ILocation destination, ReplaceMode replaceMode)
        {
            var sourceManager = LocationFactory.GetManager(source, replaceMode);
            var destinationManager = LocationFactory.GetManager(destination, replaceMode);

            var canCopy = false;
            var destinationExists = destinationManager.Exists();
            if (!destinationExists)
                canCopy = true;

            if (destinationExists && (replaceMode & ReplaceMode.ReplaceIfDifferentSize) == ReplaceMode.ReplaceIfDifferentSize)
            {
                var sourceSize = sourceManager.GetSize();
                var destinationSize = destinationManager.GetSize();

                if (sourceSize != destinationSize)
                    canCopy = true;
            }

            if (destinationExists && replaceMode == ReplaceMode.Replace)
                canCopy = true;

            if (destinationExists && replaceMode == ReplaceMode.Rename)
            {
                destination.ItemUri = new Uri(Path.Combine(destination.ItemUri.AbsolutePath,
                    Path.GetFileNameWithoutExtension(destination.ItemUri.AbsoluteUri) + "_" +
                    DateTime.Now.TimeOfDay.ToString("HHmmss") +
                    Path.GetExtension(destination.ItemUri.AbsoluteUri)));

                canCopy = true;
            }

            if (destinationExists && replaceMode == ReplaceMode.UserAsking)
            {
                if (UserAskHandler == null)
                    throw new Exception("If the ReplaceMode is ReplaceMode.UserAsking, the UserAskHandler delegate shall be set.");
                
                destinationManager.ReplaceMode = UserAskHandler();
            }

            if (canCopy)
            {
                using (var pushFile = destinationManager.BeginPushFile())
                {
                    sourceManager.BeginRetreiveFile(pushFile.BufferWriteCallback, pushFile.CopyDone);

                    pushFile.Done.WaitOne();
                }
            }

            return canCopy;
        }
    }
}
