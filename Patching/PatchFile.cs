// Copyright 2020 Ammo Goettsch
// 
// Helios is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Helios is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DiffMatchPatch;
using GadrocsWorkshop.Helios.Util;

namespace GadrocsWorkshop.Helios.Patching
{
    public class PatchFile
    {
        private static readonly diff_match_patch _googleDiff = new diff_match_patch();
        private static readonly Encoding _utf8WithoutBom = new UTF8Encoding(false);
        private List<Patch> _patches;
        private List<Patch> _reversePatches;

        public string TargetPath { get; internal set; }

        internal bool TryApply(string source, out string patched, out string status, out string expected)
        {
            status = null;
            if (_patches.Count < 1)
            {
                // don't bother running the code below with zero sized arrays
                ConfigManager.LogManager.LogInfo($"ignoring empty patch for '{Anonymizer.Anonymize(TargetPath)}'");
                expected = null;
                patched = source;
                return true;
            }

            bool[] applied = GooglePatchApply(source, _patches, out patched, out List<Patch> effectivePatches, out diff_match_patch.PatchResult[] resultCodes);
            for (int i = 0; i < applied.Length; i++)
            {
                if (!applied[i])
                {
                    status = $"failed to apply patch to {TargetPath}: {resultCodes[i]}{Environment.NewLine}The patch could not locate the expected content section (searching near character {effectivePatches[i].start1}):";
                    IEnumerable<string> expectedText = effectivePatches[i].diffs
                        .Where(diff => diff.operation != Operation.INSERT)
                        .Select(diff => diff.text);
                    expected = string.Join("", expectedText);
                    return false;
                }

                switch (resultCodes[i])
                {
                    case diff_match_patch.PatchResult.UNKNOWN:
                        throw new Exception($"invalid result code from application of {effectivePatches[i]}");
                    case diff_match_patch.PatchResult.APPLIED_PERFECT:
                        break;
                    case diff_match_patch.PatchResult.APPLIED_IMPERFECT:
                        status = "patch imperfectly matched input, but was applied";
                        break;
                    default:
                        throw new Exception(
                            $"patch should not have returned success with result code {resultCodes[i]}");
                }
            }

            expected = null;
            return true;
        }

        private bool[] GooglePatchApply(string source, List<Patch> patches, out string patched, out List<Patch> effectivePatches, out diff_match_patch.PatchResult[] resultCodes)
        {
            object[] results = _googleDiff.patch_apply(patches, source, out effectivePatches);
            if (results.Length != 3)
            {
                // something horrible has happened with out third party library call
                throw new Exception(
                    $"patch_apply in GoogleDiffMatchPatch returned incorrect number of results: {results.Length}; broken installation");
            }

            patched = (string) results[0];
            bool[] applied = (bool[]) results[1];
            resultCodes = (diff_match_patch.PatchResult[]) results[2];
            if (applied.Length != resultCodes.Length)
            {
                // something horrible has happened with out third party library call
                throw new Exception(
                    $"patch_apply in GoogleDiffMatchPatch returned unmatched result arrays; broken installation");
            }

            if (applied.Length != effectivePatches.Count)
            {
                // something horrible has happened with out third party library call
                throw new Exception(
                    $"patch_apply in GoogleDiffMatchPatch returned incorrect number of results {applied.Length} for {effectivePatches.Count} patches; broken installation");
            }

            return applied;
        }

        internal bool TryRevert(string source, out string patched, out string status)
        {
            if (_reversePatches == null)
            {
                status = $"no reverse patch available for {TargetPath}";
                patched = source;
                return false;
            }

            status = null;
            bool[] applied = GooglePatchApply(source, _reversePatches, out patched, out List<Patch> effectivePatches, out diff_match_patch.PatchResult[] resultCodes);
            for (int i = 0; i < applied.Length; i++)
            {
                if (!applied[i])
                {
                    status = $"failed to revert patch against {TargetPath}: {resultCodes[i]}";
                    return false;
                }

                switch (resultCodes[i])
                {
                    case diff_match_patch.PatchResult.UNKNOWN:
                        throw new Exception($"invalid result code from application of reverse patch {effectivePatches[i]}");
                    case diff_match_patch.PatchResult.APPLIED_PERFECT:
                        break;
                    case diff_match_patch.PatchResult.APPLIED_IMPERFECT:
                        status = "reverse patch imperfectly matched input, but was applied";
                        break;
                    default:
                        throw new Exception(
                            $"reverse patch should not have returned success with result code {resultCodes[i]}");
                }
            }

            return true;
        }

        internal bool IsApplied(string patched)
        {
            // check if applied by just seeing if all the inserts are present (we don't generate patches that insert and then delete the same thing)
            foreach (Patch patch in _patches)
            {
                string matchText = "";
                foreach (Diff chunk in patch.diffs)
                {
                    switch (chunk.operation)
                    {
                        case Operation.DELETE:
                            break;
                        case Operation.INSERT:
                        case Operation.EQUAL:
                            matchText += chunk.text;
                            break;
                    }
                }

                if (!patched.Contains(matchText))
                {
                    return false;
                }
            }

            return true;
        }

        internal void Load(string patchPath)
        {
            using (StreamReader streamReader = new StreamReader(patchPath, _utf8WithoutBom))
            {
                string patchText = streamReader.ReadToEnd();
                _patches = _googleDiff.patch_fromText(patchText);
            }

            string revertPath = Path.ChangeExtension(patchPath, Path.GetExtension(patchPath) == ".gpat" ? ".grev" : ".grevert");
            if (!File.Exists(revertPath))
            {
                return;
            }

            using (StreamReader streamReader = new StreamReader(revertPath, _utf8WithoutBom))
            {
                string patchText = streamReader.ReadToEnd();
                _reversePatches = _googleDiff.patch_fromText(patchText);
            }
        }
    }
}