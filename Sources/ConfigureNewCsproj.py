'''
This script performs modifies a newly created visual studio
project file to configure the default settings.

Warning: Call this script only once for any given project file.
Calling it again will mess up the csproj file.

'''

import sys
import os
import argparse
import shutil

from lxml import etree
from copy import deepcopy




def xpath_ns(tree, expr):
    "Parse a simple expression and prepend namespace wildcards where unspecified."
    qual = lambda n: n if not n or ':' in n else '*[local-name() = "%s"]' % n
    expr = '/'.join(qual(n) for n in expr.split('/'))
    nsmap = dict((k, v) for k, v in tree.nsmap.items() if k)
    return tree.xpath(expr, namespaces=nsmap)


def addVersionInfoFile(root):

    compileElements = xpath_ns(root, '//ItemGroup/Compile')

    if any('VersionInfo.cs' in elem.get('Include') for elem in compileElements):
        print 'VersionInfo.cs already referenced (no action required)'
    else:
        newCompileElement = etree.fromstring(r"""
            <Compile Include="..\VersionInfo.cs">
                    <Link>Properties\VersionInfo.cs</Link>
            </Compile>""")
        compileElements[0].getparent().append(newCompileElement)
        print 'VersionInfo.cs successfully referenced'


def setNamespace(root, layerName):
    elem = xpath_ns(root, '//RootNamespace')[0]

    if 'Thedmi' in elem.text:
        print 'Namespace already set (no action required)'
    else:
        namespace = 'Thedmi.DecentDonkey.' + layerName + '.' + elem.text
        elem.text = namespace
        print 'Namespace successfully set to ' + namespace
            

def setOutputPath(root):

    projectTypeGuids = xpath_ns(root, '//ProjectTypeGuids')
    
    #if len(projectTypeGuids) > 0 and '{349c5851-65df-11da-9384-00065b846f21}' in projectTypeGuids[0].text.lower():
    #    print 'Not setting output path for Web App project, as this would break debugging'
    #else:
    for elem in xpath_ns(root, '//OutputPath'):
        if '..\\' in elem.text:
            print 'OutputPath already set (no action required)'
        else:
            elem.text = '..\\' + elem.text
            print 'OutputPath successfully corrected'


def configureCodeAnalysis(root):    
    for elem in xpath_ns(root, '//PropertyGroup'):
        if elem.get('Condition'):
            if len(xpath_ns(elem, 'RunCodeAnalysis')) > 0:
                print 'Code analysis already configured (no action required)'
            else:

                if len(xpath_ns(elem, 'CodeAnalysisRuleSet')) == 0:
                    elem.append(etree.fromstring(r"<CodeAnalysisRuleSet>..\ThedmiDefaultFxCop.ruleset</CodeAnalysisRuleSet>"))
                
                elem.append(etree.fromstring(r"<RunCodeAnalysis>False</RunCodeAnalysis>"))
                    
                print 'Code analysis successfully configured'



def configureCsproj(projectName, csprojPath, layerName):
    
    tree = etree.parse(csprojPath)
    root = tree.getroot()

    addVersionInfoFile(root)
    setNamespace(root, layerName)
    setOutputPath(root)
    configureCodeAnalysis(root)

    shutil.copyfile(csprojPath, csprojPath + '.bak')
    tree.write(csprojPath)

    print 'Project ' + projectName + ' successfully configured'



def main():
    
    parser = argparse.ArgumentParser()
    parser.add_argument(dest='projectName')
    parser.add_argument(dest='layerName')

    args = parser.parse_args()
    projectName = args.projectName
    layerName = args.layerName
    
    scriptDir = sys.path[0]
    workingDir = os.path.normpath(os.path.join(scriptDir, '../Sources'))
    csprojPath = os.path.normpath(os.path.join(projectName, projectName + '.csproj'))

    os.chdir(workingDir)
    
    print 'Working directory is ' + os.getcwd()

    if os.path.exists(csprojPath):
        configureCsproj(projectName, csprojPath, layerName)
    else:
        raise Exception('Specified project not found')
    

    
        
if __name__ == '__main__':
    main()
    
